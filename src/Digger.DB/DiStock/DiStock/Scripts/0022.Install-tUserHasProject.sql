create table dbo.tUserHasProject
(
	FktUser int not null, 
	FktProject int not null,
	FktStaticProjectAccessRight int not null

	constraint FK_tUserHasProject_tUser foreign key(FktUser) references dbo.tUser(Id),
	constraint FK_tUserHasProject_tProject foreign key (FktProject) references dbo.tProject(Id),
	constraint FK_tUserHasProject_tStaticProjectAccessRight foreign key (FktStaticProjectAccessRight) references dbo.tStaticProjectAccessRight(Id)
);