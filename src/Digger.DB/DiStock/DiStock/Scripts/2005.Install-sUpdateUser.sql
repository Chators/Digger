create proc dbo.sUpdateUser
(
    @Id				int,
	@Pseudo			nvarchar(50),
    @Password		varbinary(120),
	@Role			nvarchar(50)
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tUser u where u.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	if exists(select * from dbo.tUser u where u.Id <> @Id and u.Pseudo = @Pseudo)
	begin
		rollback;
		return 2;
	end;

    update dbo.tUser set [Pseudo] = @Pseudo, [Password] = @Password, [Role] = @Role where Id = @Id;
	commit;
    return 0;
end;