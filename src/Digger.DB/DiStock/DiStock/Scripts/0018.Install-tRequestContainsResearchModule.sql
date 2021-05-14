create table dbo.tRequestContainsResearchModule
(
FktResearchModule int not null, 
FktRequest int not null

constraint FK_tRequestContainsResearchModule_tResearchModule foreign key(FktResearchModule) references dbo.tResearchModule(Id),
constraint FK_tRequestContainsResearchModule_tRequest foreign key (FktRequest) references dbo.tRequest(Id)
);


