create table dbo.tRequest
(
	Id int identity(1, 1) not null,
	FktProject int not null,
	FktStaticStatusRequest int not null,
	DataEntity nvarchar(255) not null,
	UidNode nvarchar(255) not null,
	Author nvarchar(255) not null,
	Date datetime not null
	
	constraint PK_tRequest primary key(Id),
	constraint FK_tRequest_tProject foreign key (FktProject) references dbo.tProject(Id),
	constraint FK_tRequest_tStaticStatusRequest foreign key (FktStaticStatusRequest) references dbo.tStaticStatusRequest(Id)
);