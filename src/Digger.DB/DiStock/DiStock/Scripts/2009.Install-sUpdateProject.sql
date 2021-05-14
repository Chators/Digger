create proc dbo.sUpdateProject
(
    @Id				int,
    @Name			nvarchar(255),
	@Description	nvarchar(255),
	@IsPublic		bit
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if not exists(select * from dbo.tProject p where p.Id = @Id)
	begin
		rollback;
		return 1;
	end;

	if exists(select * from dbo.tProject p where p.Name = @Name)
	begin
		rollback;
		return 2;
	end;

    update dbo.tProject set Name = @Name, Description = @Description, IsPublic = @IsPublic where Id = @Id;
	commit;
    return 0;
end;