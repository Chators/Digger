create proc dbo.sUpdateAccessRightUserInProject
(
	@FktUser							int,
	@FktProject							int,
	@FktStaticProjectAccessRight		int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tUserHasProject uhp where uhp.FktUser = @FktUser AND uhp.FktProject = @FktProject)
	begin
		rollback;
		return 1;
	end;

    update dbo.tUserHasProject set FktStaticProjectAccessRight = @FktStaticProjectAccessRight where FktUser = @FktUser AND FktProject = @FktProject;

	commit;
    return 0;
end;