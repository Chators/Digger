create proc [dbo].[sCreateProject]
(
	@FktUser int,
    @Name  nvarchar(255),
	@Description nvarchar(255),
	@Date datetime,
	@IsPublic bit,
	@IdProjectAccessRight int,
	@Id int out
)
as
begin
	set transaction isolation level serializable;
	begin tran;

	if exists(select * from dbo.tProject p where p.Name = @Name)
	begin
		rollback;
		return 1;
	end;

    insert into dbo.tProject (Name, Description, Date, IsPublic) values(@Name, @Description, @Date, @IsPublic);
	set @Id = scope_identity();
	insert into dbo.tUserHasProject (FktUser, FktProject, FktStaticProjectAccessRight) values (@FktUser, @Id, @IdProjectAccessRight);
	commit;
    return 0;
end;
GO