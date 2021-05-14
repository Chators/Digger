create view [dbo].[vUserInvitationInProject]
	as
select
	u.Id as UserAuthorId,
	u.Pseudo as UserAuthorPseudo,
	ui.Id as UserInvitedId,
	ui.Pseudo as UserInvitedPseudo,
	p.Id as ProjectId,
	p.Name as ProjectName
from dbo.tUserInvitationInProject uiip
	inner join dbo.tUser u on uiip.FktUserAuthor = u.Id
	inner join dbo.tUser ui on uiip.FktUserInvited = ui.Id
	inner join dbo.tProject p on uiip.FktProject = p.Id
GO