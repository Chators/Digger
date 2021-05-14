create proc dbo.sChangeStatusRequest
(
	@Id							int,
	@FktStaticStatusRequest		int
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tRequest r where r.Id = @Id)
	begin
		rollback;
		return 1;
	end;

    update dbo.tRequest set FktStaticStatusRequest = @FktStaticStatusRequest where Id = @Id;

	commit;
    return 0;
end;