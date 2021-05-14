create proc dbo.sCreateUserInvitationInProject
(
    @UserAuthorId   int,
    @UserInvitedId  int,
	@ProjectId		int
)
as
begin
    set transaction isolation level serializable;
begin tran;

	if exists(select * from dbo.tUserInvitationInProject uiip where uiip.FktUserAuthor = @UserAuthorId AND uiip.FktUserInvited = @UserInvitedId AND uiip.FktProject = @ProjectId)
    begin
        rollback;
		return 1;
	end;

    insert into dbo.tUserInvitationInProject (FktUserAuthor, FktUserInvited, FktProject) values(@UserAuthorId, @UserInvitedId, @ProjectId);

commit;
    return 0;
end;