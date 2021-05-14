create table dbo.tUserInvitationInProject
(
	FktUserInvited int not null, 
	FktProject int not null,
	FktUserAuthor int not null
	

	constraint FK_tUserInvitationInProject_tUser_FktUserInvited foreign key(FktUserInvited) references dbo.tUser(Id),
	constraint FK_tUserInvitationInProject_tUser_FktUserAuthor foreign key(FktUserAuthor) references dbo.tUser(Id),
	constraint FK_tUserInvitationInProject_tProject foreign key (FktProject) references dbo.tProject(Id)
);