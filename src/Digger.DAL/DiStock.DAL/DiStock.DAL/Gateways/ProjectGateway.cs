using Dapper;
using DiStock.DAL.Datas;
using DiStock.DAL.Datas.Project;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiStock.DAL
{
    public class ProjectGateway
    {
        readonly string _connectionString;

        public ProjectGateway(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ProjectData>> GetAllProject()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectData>(
                    @"select p.Id,
                             p.Name,
                             p.Description,
                             p.Date,
                             p.IsPublic
                      from dbo.tProject p;");
            }
        }

        public async Task<IEnumerable<ProjectData>> GetProjectPublicByUserId(int userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectData>(
                    @"select p.Id,
                             p.Name,
                             p.Description,
                             p.Date,
                             p.IsPublic
                      from tProject p
                      left join tUserHasProject uhp on uhp.FktUser = @UserId AND  p.Id = uhp.FktProject
                      where IsPublic = 1 AND uhp.FktProject IS NULL",
                    new { UserId = userId });
            }
        }

        public async Task<IEnumerable<string>> GetIdUserInProject(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<string>(
                    @"select distinct UserId 
                      from vUser 
                      where ProjectId = @ProjectId;",
                    new { ProjectId = projectId });
            }
        }

        public async Task<IEnumerable<ProjectAccessRightData>> GetStaticProjectAccessRight()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectAccessRightData>(
                    @"select spar.Id,
                             spar.AccessRight
                      from tStaticProjectAccessRight spar
                      order by spar.Id;");
            }
        }

        public async Task<string> GetProjectName(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<string>(
                    @"select p.Name
                      from dbo.tProject p
                      where p.Id = @ProjectId;",
                    new { ProjectId = projectId });
            }
        }

        public async Task<Result<ProjectData>> GetProjectById(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                ProjectData project = await con.QueryFirstOrDefaultAsync<ProjectData>(
                    @"select p.Id,
                             p.Name,
                             p.Description,
                             p.Date,
                             p.IsPublic
                      from dbo.tProject p
                      where p.Id = @ProjectId;",
                    new { ProjectId = projectId });

                if (project == null) return Result.Failure<ProjectData>(HttpStatusCode.NotFound, "Project not found");

                return Result.Success(HttpStatusCode.OK, project);
            }
        }

        public async Task<IEnumerable<ProjectUserIdData>> GetProjectByUserId(int userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectUserIdData>(
                    @"select distinct u.ProjectId as Id,
                                      u.UserAccessRightProject as AccessRight,
                                      u.ProjectName as Name,
                                      u.ProjectDescription as Description,
                                      u.ProjectDate as Date,
                                      u.ProjectIsPublic as IsPublic
                      from vUser u
                      where u.UserId = @UserId",
                      new { UserId = userId });
            }
        }

        public async Task<ProjectIsPublic> ProjectIsPublic(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<ProjectIsPublic>(
                    @"select p.IsPublic
                      from tProject p
                      where p.Id = @ProjectId AND p.IsPublic = 1",
                      new { @ProjectId = projectId });
            }
        }

        public async Task<IEnumerable<ProjectInvitationData>> GetUserInvitationByUserId(int userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectInvitationData>(
                    @"select uiip.UserAuthorId,
                             uiip.UserAuthorPseudo,
                             uiip.UserInvitedId,
                             uiip.UserInvitedPseudo,
                             uiip.ProjectId,
                             uiip.ProjectName
                      from vUserInvitationInProject uiip
                      where uiip.UserInvitedId = @UserId",
                      new { UserId = userId });
            }
        }

        public async Task<int> CheckUserInvitation(int userId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<int>(
                    @"select Count(*)
                      from vUserInvitationInProject uiip
                      where uiip.UserInvitedId = @UserId AND uiip.ProjectId = @ProjectId",
                      new { UserId = userId, ProjectId = projectId });
            }
        }

        public async Task<int> CheckUserInProject(int userId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryFirstOrDefaultAsync<int>(
                    @"select count(*)
                      from tUserHasProject 
                      where FktUser = @UserId AND FktProject = @ProjectId",
                      new { UserId = userId, ProjectId = projectId });
            }
        }

        public async Task<IEnumerable<ProjectInvitationData>> GetAuthorInvitationByUserId(int userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<ProjectInvitationData>(
                    @"select uiip.UserAuthorId,
                             uiip.UserAuthorPseudo,
                             uiip.UserInvitedId,
                             uiip.UserInvitedPseudo,
                             uiip.ProjectId,
                             uiip.ProjectName
                      from vUserInvitationInProject uiip
                      where uiip.UserAuthorId = @UserId",
                      new { UserId = userId });
            }
        }
        
        public async Task<Result<string>> GetUserAccessRightProject(int userId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string r = await con.QueryFirstOrDefaultAsync<string>(
                    @"select distinct UserAccessRightProject 
                      from vUser 
                      where UserId = @UserId AND ProjectId = @ProjectId;"
                      , new { UserId = userId, ProjectId = projectId });

                return Result.Success(HttpStatusCode.OK, r);
            }
        }

        public async Task<int> GetNumberAccessRightInProject(int projectId, int accessRightId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                int r = await con.QueryFirstOrDefaultAsync<int>(
                    @"select count(*)
                      from tUserHasProject 
                      where FktProject = @ProjectId AND FktStaticProjectAccessRight = @AccessRightId;"
                      , new { ProjectId = projectId, AccessRightId = accessRightId });

                return r;
            }
        }

        public async Task<IEnumerable<UserInProjectData>> GetUserInProject(int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                return await con.QueryAsync<UserInProjectData>(
                    @"select distinct UserId, UserPseudo, UserAccessRightProject 
                      from vUser 
                      where ProjectId = @ProjectId",
                      new { ProjectId = projectId });
            }
        }

        public async Task<Result<int>> CreateProject(int fktUser, string name, string description, int idProjectAccessRight, byte isPublic)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktUser", fktUser);
                p.Add("@Name", name);
                p.Add("@Description", description); 
                p.Add("@Date", DateTime.Now);
                p.Add("@IdProjectAccessRight", idProjectAccessRight);
                p.Add("@IsPublic", isPublic);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "Project with this name already exists");

                return Result.Success(HttpStatusCode.Created, p.Get<int>("@id"));
            }
        }

        public async Task<Result> UpdateProject(int id, string name, string description, byte isPublic)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Name", name);
                p.Add("@Description", description);
                p.Add("@IsPublic", isPublic);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sUpdateProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "Project with this id not exists");
                if (status == 2) return Result.Failure<int>(HttpStatusCode.BadRequest, "Project with this name already exists");
                
                return Result.Success(status);
            }
        }

        public async Task<Result> UpdateAccessRightUserInProject(int userId, int projectId, int accessRightId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktUser", userId);
                p.Add("@FktProject", projectId);
                p.Add("@FktStaticProjectAccessRight", accessRightId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sUpdateAccessRightUserInProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure<int>(HttpStatusCode.BadRequest, "User not belongs in this project");

                return Result.Success(status);
            }
        }

        public async Task<Result> DeleteProject(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@Id", id);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.NotFound, "Project not found");

                return Result.Success(HttpStatusCode.OK, true);
            }
        }

        public async Task<Result> AssignUserToProject(int userId, int projectId, int accessRightId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktUser", userId);
                p.Add("@FktProject", projectId);
                p.Add("@FktStaticProjectAccessRight", accessRightId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sAssignUserToProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "The project is already assigned to user");

                return Result.Success(status);
            }
        }

        public async Task<Result> CreateUserInvitationInProject(int userAuthorId, int userInvitedId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UserAuthorId", userAuthorId);
                p.Add("@UserInvitedId", userInvitedId);
                p.Add("@ProjectId", projectId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sCreateUserInvitationInProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "An invitation made by the user with this project and user invited already exist");

                return Result.Success(status);
            }
        }

        public async Task<Result> DeleteUserInvitationInProjectByUserInvitedAndProject(int userInvitedId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UserInvitedId", userInvitedId);
                p.Add("@ProjectId", projectId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteUserInvitationInProjectByUserInvitedAndProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "No invitation with this user id and project id exists");

                return Result.Success(status);
            }
        }

        public async Task<Result> DeleteUserInvitationInProject(int userAuthorId, int userInvitedId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@UserAuthorId", userAuthorId);
                p.Add("@UserInvitedId", userInvitedId);
                p.Add("@ProjectId", projectId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sDeleteUserInvitationInProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "No invitation with this user author id, user invited id and project id exists");

                return Result.Success(status);
            }
        }

        public async Task<Result> UnassignUserToProject(int userId, int projectId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                var p = new DynamicParameters();
                p.Add("@FktUser", userId);
                p.Add("@FktProject", projectId);
                p.Add("@Status", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await con.ExecuteAsync("dbo.sUnassignUserToProject", p, commandType: CommandType.StoredProcedure);

                int status = p.Get<int>("@Status");

                if (status == 1) return Result.Failure(HttpStatusCode.BadRequest, "User is not in the project");

                return Result.Success(status);
            }
        }
    }
}
