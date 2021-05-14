create proc [dbo].[sCreateTypeEntity]
(
	@TypeEntity nvarchar(255)
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tStaticEntity se where se.TypeEntity = @TypeEntity)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tStaticEntity (TypeEntity) values(@TypeEntity);
	commit;
    return 0;
end;
GO