create proc dbo.sUpdateResearchModule
(
	@Id						int,
	@FktStaticEntity		int,
	@FktStaticFootprint		int,
	@Name					nvarchar(100),
	@Description			nvarchar(255)
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tResearchModule rm where rm.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	if exists(select * from dbo.tResearchModule rm where rm.Id <> @Id and [Name] = @Name)
	begin
		rollback;
		return 2;
	end;

    update dbo.tResearchModule set FktStaticEntity = @FktStaticEntity, FktStaticFootprint =	@FktStaticFootprint, [Name] = @Name, [Description] = @Description where Id = @Id;
	commit;
    return 0;
end;