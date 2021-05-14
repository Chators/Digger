create proc [dbo].[sCreateUser]
(
    @Pseudo nvarchar(50),
    @Password  varbinary(120),
	@Role nvarchar(50),
	@Id int out
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tUser u where u.Pseudo = @Pseudo)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tUser (Pseudo, Password, Role) values(@Pseudo, @Password, @Role);
	set @Id = scope_identity();
	commit;
    return 0;
end;
GO