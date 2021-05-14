create table dbo.tStaticEntity
(
	Id int identity(1, 1) not null,
    TypeEntity nvarchar(100) not null

	constraint PK_tStaticEntity primary key(Id),
);
