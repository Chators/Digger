using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using DiStock.DAL.Datas;

namespace DiStock.DAL
{
    public class SoftwareGateway
    {
        readonly string _connectionString;

        public SoftwareGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<SoftwareData>> GetAllSoftware()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<SoftwareData>(
                    @"select s.Id,
                             s.Name,
                             s.Description,
                             s.AverageExecutionTime
                      from dbo.tSoftware s;");
            }
        }

        public async Task<Result<SoftwareData>> GetSoftwareById(int softwareId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SoftwareData software = await con.QueryFirstOrDefaultAsync<SoftwareData>(
                    @"select s.Id,
                             s.Name,
                             s.Description,
                             s.AverageExecutionTime
                      from dbo.tSoftware s
                      where s.Id = @SoftwareId;",
                    new { SoftwareId = softwareId });
                
                return Result.Success(HttpStatusCode.OK, software);
            }
            
        }
        
        public async Task<Result<int>> CreateSoftware ( string name, string description )
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@name", name);
                p.Add("@description", description);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateSoftware", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "Software with this name exists");

                return Result.Success(HttpStatusCode.Created, p.Get<int>("@id"));
            }
        }
        
        public async Task<Result> DeleteSoftware ( int id )
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteSoftware", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "Software not found");

                return Result.Success(HttpStatusCode.OK, true);
            }
        }

        public async Task<Result> UpdateSoftware (int id, string name, string description)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Name", name);
                p.Add("@Description", description);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sUpdateSoftware", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "Software not found");
                if (status == 2) return Result.Failure(HttpStatusCode.BadRequest, "Software with this name already exists");

                return Result.Success(status);
            }
        }
    }
}
