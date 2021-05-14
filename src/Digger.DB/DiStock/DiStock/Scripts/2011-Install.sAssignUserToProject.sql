create proc [dbo].[sAssignUserToProject]
(
    @FktUser int,
    @FktProject int,
	@FktStaticProjectAccessRight int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tUserHasProject uhp where uhp.FktUser = @FktUser AND uhp.FktProject = @FktProject)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tUserHasProject (FktUser, FktProject, FktStaticProjectAccessRight) values(@FktUser, @FktProject, @FktStaticProjectAccessRight);

	commit;
    return 0;
end;
GO