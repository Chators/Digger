create proc dbo.sDeleteRequest
(
    @Id   int
)
as
begin
    set transaction isolation level serializable;
begin tran;

	if not exists(select * from dbo.tRequest where Id = @Id)
    begin
        rollback;
		return 1;
	end;

	delete dbo.tRequestContainsResearchModule where FktRequest = @Id;
	delete dbo.tRequest where Id = @Id;

commit;
    return 0;
end;