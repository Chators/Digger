create view [dbo].[vUser]
	as
select
	u.Id as UserId,
	u.Pseudo as UserPseudo,
	uhp.AccessRight as UserAccessRightProject,
	p.Id as ProjectId,
	p.Name as ProjectName,
	p.Description as ProjectDescription,
	p.Date as ProjectDate,
	p.IsPublic as ProjectIsPublic
from dbo.tUser u
	inner join dbo.vUserHasProject uhp on u.Id = uhp.UserId
	left join dbo.tProject p on uhp.ProjectId = p.Id
where u.Id <> 0;
GO
