create table dbo.tStaticFootprint
(
	Id int identity(1, 1) not null,
    LevelFootprint nvarchar(100) not null

	constraint PK_tStaticFootprint primary key(Id),
);

