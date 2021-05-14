create proc dbo.sCreateRequest
(
    @FktStaticStatusRequest   int,
	@FktProject				  int,
    @DataEntity				  nvarchar(255),
	@UidNode				  nvarchar(255),
	@Author					  nvarchar(255),
	@Date					  datetime,
	@Id						  int out
)
as
begin
    set transaction isolation level serializable;
begin tran;

    insert into dbo.tRequest (FktStaticStatusRequest, FktProject, DataEntity, UidNode, Author, Date) values(@FktStaticStatusRequest, @FktProject, @DataEntity, @UidNode, @Author, @Date);
	set @Id = scope_identity();
commit;
    return 0;
end;