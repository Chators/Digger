create proc dbo.sUpdateSoftware
(
    @Id				int,
    @Name			nvarchar(50),
    @Description    nvarchar(255)
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

	if exists(select * from dbo.tSoftware s where s.Id <> @Id and s.[Name] = @Name)
	begin
		rollback;
		return 2;
	end;

    update dbo.tSoftware set [Name] = @Name, [Description] = @Description where Id = @Id;
	commit;
    return 0;
end;