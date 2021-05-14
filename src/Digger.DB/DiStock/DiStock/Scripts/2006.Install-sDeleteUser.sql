create proc dbo.sDeleteUser
(
    @Id   int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tUser u where u.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	delete dbo.tUserInvitationInProject where FktUserInvited = @Id OR FktUserAuthor = @Id;
    delete dbo.tUserHasProject where FktUser = @Id;
    delete dbo.tUser where Id = @Id;

	commit;
    return 0;
end;