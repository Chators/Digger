create proc dbo.sDeleteUserInvitationInProjectByUserInvitedAndProject
(
    @UserInvitedId   int,
	@ProjectId int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tUserInvitationInProject uiip where uiip.FktUserInvited = @UserInvitedId AND uiip.FktProject = @ProjectId)
	begin
		rollback;
		return 1;
	end;

	delete dbo.tUserInvitationInProject where FktUserInvited = @UserInvitedId AND FktProject = @ProjectId;
	
	commit;
    return 0;
end;