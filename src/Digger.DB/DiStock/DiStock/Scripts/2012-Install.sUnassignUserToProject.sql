create proc [dbo].[sUnassignUserToProject]
(
    @FktUser int,
    @FktProject int
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

    delete dbo.tUserHasProject where FktUser = @FktUser AND FktProject = @FktProject;

	commit;
    return 0;
end;
GO