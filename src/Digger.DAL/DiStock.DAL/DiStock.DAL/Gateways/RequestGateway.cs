using Dapper;
using DiStock.DAL.Datas;
using DiStock.DAL.Datas.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace DiStock.DAL
{
    public class RequestGateway
    {
        readonly string _connectionString;

        public RequestGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<RequestData> GetRequestById(int requestId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<RequestData>(
                    @"select r.RequestId as Id,
                             r.RequestProjectId as ProjectId,
                             r.RequestDataEntity as DataEntity,
                             r.RequestUidNode as UidNode,
                             r.RequestAuthor as Author,
                             r.RequestDate as Date,
                             r.RequestStatus as Status
                      from dbo.vRequest r
                      where r.RequestId = @RequestId;",
                    new { RequestId = requestId });
            }
        }

        public async Task<IEnumerable<RequestByProjectIdData>> GetRequestByProjectId(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<RequestByProjectIdData>(
                    @"select r.RequestId as Id,
                             r.RequestDataEntity as DataEntity,
                             r.RequestUidNode as UidNode,
                             r.RequestAuthor as Author,
                             r.RequestDate as Date,
                             r.RequestStatus as Status
                      from vRequest r 
                      where RequestProjectId = @ProjectId",
                    new { ProjectId = projectId });
            }
        }

        public async Task<IEnumerable<string>> GetTypeEntity()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<string>(
                    @"select se.TypeEntity
                      from tStaticEntity se");
            }
        }

        public async Task<Result<int>> CreateRequest(int fktStaticStatusRequest, int fktProject, string dataEntity, string uidNode, string author)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktStaticStatusRequest", fktStaticStatusRequest);
                p.Add("@FktProject", fktProject);
                p.Add("@UidNode", uidNode);
                p.Add("@Author", author);
                p.Add("@DataEntity", dataEntity);
                p.Add("@Date", DateTime.Now);
                p.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await con.ExecuteAsync("dbo.sCreateRequest", p, commandType: CommandType.StoredProcedure);
                
                return Result.Success(HttpStatusCode.Created, p.Get<int>("@Id"));
            }
        }

        public async Task<Result<bool>> DeleteRequest(int requestId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", requestId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteRequest", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<bool>(HttpStatusCode.BadRequest, "Request not found");

                return Result.Success(HttpStatusCode.Accepted, true);
            }
        }

        public async Task<Result<bool>> ChangeStatusRequest(int id, int fktStaticStatusRequest)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@FktStaticStatusRequest", fktStaticStatusRequest);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sChangeStatusRequest", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<bool>(HttpStatusCode.BadRequest, "Request not found");

                return Result.Success(HttpStatusCode.Accepted, true);
            }
        }
    }
}
