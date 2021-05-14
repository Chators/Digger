create proc [dbo].[sCreateSoftware]
(
    @Name nvarchar(50),
    @Description  nvarchar(255),
	@Id int out
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tSoftware t where t.Name = @Name)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tSoftware (name, description) values(@Name, @Description);
	set @Id = scope_identity();
	commit;
    return 0;
end;
GO