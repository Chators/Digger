create proc dbo.sDeleteSoftware
(
    @Id   int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tSoftware s where s.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	while (select Id from tResearchModule where FktSoftware = @Id order by FktSoftware offset 0 rows fetch next 1 rows only) IS NOT NULL 
	begin  
		while (select rcrm.FktRequest from tRequestContainsResearchModule rcrm where rcrm.FktResearchModule = (select Id from tResearchModule where FktSoftware = @Id order by FktSoftware offset 0 rows fetch next 1 rows only)) IS NOT NULL
		begin
			delete from tRequestContainsResearchModule where FktRequest = (select rcrm.FktRequest from tRequestContainsResearchModule rcrm where rcrm.FktResearchModule = (select Id from tResearchModule where FktSoftware = @Id order by FktSoftware offset 0 rows fetch next 1 rows only));
			delete from tRequest where Id = (select rcrm.FktRequest from tRequestContainsResearchModule rcrm where rcrm.FktResearchModule = (select Id from tResearchModule where FktSoftware = @Id order by FktSoftware offset 0 rows fetch next 1 rows only));
		end;
    delete from dbo.tResearchModule where Id = (select Id from tResearchModule where FktSoftware = @Id order by FktSoftware offset 0 rows fetch next 1 rows only);
	end;
    delete from dbo.tSoftware where Id = @Id;

	commit;
    return 0;
end;