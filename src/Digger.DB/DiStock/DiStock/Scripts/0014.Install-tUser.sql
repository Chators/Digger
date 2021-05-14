create table dbo.tUser
(
	Id int identity(1, 1) not null,
	Pseudo nvarchar(50) not null,
	Password varbinary(120) not null,
	Role nvarchar(50), 

	constraint PK_tUser primary key(Id),
	constraint UK_tStudent_FirstName_LastName unique(Pseudo)
);