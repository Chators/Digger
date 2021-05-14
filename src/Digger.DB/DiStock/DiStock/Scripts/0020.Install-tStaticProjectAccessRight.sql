create table dbo.tStaticProjectAccessRight
(
	Id int identity(1, 1) not null,
    AccessRight nvarchar(100) not null

	constraint PK_tStaticProjectAccessRight primary key(Id),
);