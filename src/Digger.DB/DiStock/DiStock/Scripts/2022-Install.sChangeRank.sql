create proc dbo.sChangeRank
(
    @Id				int,
	@Rank			nvarchar(50)
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

    update dbo.tUser set Role = @Rank where Id = @Id;
	commit;
    return 0;
end;