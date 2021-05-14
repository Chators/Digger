using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace DiStock.DAL
{
    public class ResearchModuleGateway
    {
        readonly string _connectionString;

        public ResearchModuleGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Result<InfoFormResearchModuleData>> GetInfoFormResearchModule()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                InfoFormResearchModuleData infoForm = new InfoFormResearchModuleData();

                IEnumerable<string> e = await con.QueryAsync<string>(
                    @"select e.TypeEntity
                      from dbo.tStaticEntity e");
                infoForm.TypeEntity = e;

                IEnumerable<string> f = await con.QueryAsync<string>(
                    @"select f.LevelFootprint
                      from dbo.tStaticFootprint f");
                infoForm.LevelFootprint = f;

                return Result.Success(HttpStatusCode.OK, infoForm);
            }
        }

        public async Task<IEnumerable<ResearchModuleData>> GetResearchModuleBySoftwareId( int idSoftware )
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                IEnumerable<ResearchModuleData> r = await con.QueryAsync<ResearchModuleData>(
                    @"select s.ResearchModuleId as Id,
                             s.ResearchModuleName as Name,
                             s.ResearchModuleDescription as Description,
                             s.ResearchModuleAverageExecutionTime as AverageExecutionTime,
                             s.ResearchModuleLevelFootprint as LevelFootprint,
                             s.ResearchModuleTypeEntity as TypeEntity
                      from dbo.vSoftware s
                      where s.SoftwareId = @idSoftware;"
                      , new { idSoftware } );

                return r;
            }
        }

        public async Task<Result<ResearchModuleData>> GetResearchModuleById(int researchModuleId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ResearchModuleData r = await con.QueryFirstOrDefaultAsync<ResearchModuleData>(
                    @"select r.ResearchModuleId as Id,
                             r.ResearchModuleName as Name,
                             r.ResearchModuleDescription as Description,
                             r.ResearchModuleAverageExecutionTime as AverageExecutionTime,
                             r.ResearchModuleLevelFootprint as LevelFootprint,
                             r.ResearchModuleTypeEntity as TypeEntity
                      from dbo.vResearchModule r
                      where r.ResearchModuleId = @researchModuleId;"
                      , new { researchModuleId });

                return Result.Success(HttpStatusCode.OK, r);
            }
        }

        public async Task<Result> AddTypeEntity(string typeEntity)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@TypeEntity", typeEntity);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateTypeEntity", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "Type Entity already exists");

                return Result.Success(HttpStatusCode.Created);
            }
        }

        public async Task<Result<int>> CreateResearchModule(int fktSoftware, int fktStaticEntity, int fktStaticFootprint, string name, string description)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktSoftware", fktSoftware);
                p.Add("@FktStaticEntity", fktStaticEntity);
                p.Add("@FktStaticFootprint", fktStaticFootprint);
                p.Add("@Name", name);
                p.Add("@Description", description);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateResearchModule", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "Research Module already exists");

                return Result.Success(HttpStatusCode.Created, p.Get<int>("@id"));
            }
        }

        public async Task<Result> UpdateResearchModuleById(int idResearch, int fktStaticEntity, int fktStaticFootprint, string name, string description)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", idResearch);
                p.Add("@FktStaticEntity", fktStaticEntity);
                p.Add("@FktStaticFootprint", fktStaticFootprint);
                p.Add("@Name", name);
                p.Add("@Description", description);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("sUpdateResearchModule", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "Research Module not found");
                if (status == 2) return Result.Failure(HttpStatusCode.BadRequest, "Research Module with this name already exists");

                return Result.Success();
            }
        }

        public async Task<Result> DeleteResearchModuleById(int idResearch)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", idResearch);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("sDeleteResearchModule", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "Research Module not found");

                return Result.Success();
            }
        }
    }
}
