create proc [dbo].[sCreateResearchModule]
(
    @FktSoftware int,
    @FktStaticEntity int,
	@FktStaticFootprint int,
	@Name nvarchar(255),
	@Description nvarchar(255),
	@Id int out
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tResearchModule rm where rm.Name = @Name AND rm.FktSoftware = @FktSoftware)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tResearchModule (FktSoftware, FktStaticEntity, FktStaticFootprint, Name, Description) values(@FktSoftware, @FktStaticEntity, @FktStaticFootprint, @Name, @Description);
	set @Id = scope_identity();
	commit;
    return 0;
end;
GO