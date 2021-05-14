create table dbo.tResearchModule
(
    Id int identity(1, 1) not null,
	FktSoftware int not null,
	FktStaticEntity int not null,
	FktStaticFootprint int not null,
    Name nvarchar(100) not null,
    Description nvarchar(255) not null,
	AverageExecutionTime int null,
	

    constraint PK_tResearchModule primary key(Id),
    constraint FK_tResearchModule_tSoftware foreign key(FktSoftware) references dbo.tSoftware(Id),
	constraint FK_tResearchModule_tStaticEntity foreign key(FktStaticEntity) references dbo.tStaticEntity(Id),
	constraint FK_tResearchModule_tStaticFootprint foreign key(FktStaticFootprint) references dbo.tStaticFootprint(Id)
);