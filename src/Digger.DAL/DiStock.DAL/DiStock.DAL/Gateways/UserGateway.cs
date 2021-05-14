using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using DiStock.DAL.Datas.User;

namespace DiStock.DAL
{
    public class UserGateway
    {
        readonly string _connectionString;

        public UserGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<UserData>> GetAllUsers()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<UserData>(
                    @"select u.Id,
                             u.Pseudo,
                             u.Password,
                             u.Role
                      from dbo.tUser u;");
            }
        }

        public async Task<IEnumerable<UserForInvitData>> GetUserForInvitByProjectId(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<UserForInvitData>(
                    @"select u.Id, 
                             u.Pseudo as value
                             from tUser u
                             left join tUserHasProject uhp on uhp.FktProject = @ProjectId AND  u.Id = uhp.FktUser
                             where FktProject IS NULL",
                    new { ProjectId = projectId });
            }
        }

        public async Task<string> GetPseudoById(int userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<string>(
                    @"select u.Pseudo
                      from dbo.tUser u
                      where u.Id = @userId;",
                    new { UserId = userId });
            }
        }

        public async Task<Result<UserData>> GetUserByPseudo(string userPseudo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                UserData user = await con.QueryFirstOrDefaultAsync<UserData>(
                    @"select u.Id,
                             u.Pseudo,
                             u.Password,
                             u.Role
                      from dbo.tUser u
                      where u.Pseudo = @UserPseudo;",
                    new { UserPseudo = userPseudo });

                if (user == null) return Result.Failure<UserData>(HttpStatusCode.BadRequest, "No user with this pseudo exists");

                return Result.Success(HttpStatusCode.OK, user);
            }
        }

        public async Task<Result<int>> CreateUser(string pseudo, byte[] passwordHash, string role)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Pseudo", pseudo);
                p.Add("@Password", passwordHash);
                p.Add("@Role", role);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateUser", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "This pseudo is already used");

                return Result.Success(HttpStatusCode.Created, p.Get<int>("@id"));
            }
        }

        public async Task<Result> DeleteUser(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteUser", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "User not found");

                return Result.Success(HttpStatusCode.OK, true);
            }
        }

        public async Task<Result> ChangeRank(int userId, string rank)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", userId);
                p.Add("@Rank", rank);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sChangeRank", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "User not found");

                return Result.Success("success");
            }
        }

        public async Task<Result> UpdateUser(int id, string pseudo, byte[] passwordHash, string role)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Pseudo", pseudo);
                p.Add("@Password", passwordHash);
                p.Add("@Role", role);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sUpdateUser", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "User id does not exists");
                if (status == 2) return Result.Failure<int>(HttpStatusCode.BadRequest, "This pseudo is already used");

                return Result.Success(status);
            }
        }
    }
}
