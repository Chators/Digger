create proc dbo.sDeleteProject
(
    @Id   int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tProject p where p.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	while (select Id from tRequest where FktProject = @Id order by Id offset 0 rows fetch next 1 rows only) IS NOT NULL
	begin
		delete from tRequestContainsResearchModule where FktRequest = (select Id from tRequest where FktProject = @Id order by Id offset 0 rows fetch next 1 rows only);
		delete from tRequest where Id = (select Id from tRequest where FktProject = @Id order by Id offset 0 rows fetch next 1 rows only);
	end;
	delete dbo.tUserInvitationInProject where FktProject = @Id;
	delete dbo.tUserHasProject where FktProject = @Id;
	delete dbo.tProject where Id = @Id;
	
	commit;
    return 0;
end;