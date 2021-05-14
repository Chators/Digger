create table dbo.tStaticStatusRequest
(
	Id int identity (1, 1) not null,
	StatusRequest nvarchar(100) not null, 
	
	constraint PK_tStaticStatusRequest primary key(Id)
);
