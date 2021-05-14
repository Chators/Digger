create table dbo.tProject
(
	Id int identity(1, 1) not null,
	Name nvarchar(255) not null,
	Description nvarchar(255),
	Date datetime not null,
	IsPublic bit default 0

	constraint PK_tProject primary key(Id)
)
;
