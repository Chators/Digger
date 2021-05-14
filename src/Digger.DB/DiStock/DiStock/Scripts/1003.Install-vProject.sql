create view [dbo].[vProject]
	as
select
	p.Id as ProjectId,
	p.Name as ProjectName,
	p.Description as ProjectDescription,
	p.Date as ProjectDate,
	p.IsPublic as ProjectIsPublic,
	r.RequestId as RequestId,
	r.RequestStatus as RequestStatus,
	r.RequestDataEntity as RequestDataEntity,
	r.RequestUidNode as RequestUidNode,
	r.RequestAuthor as RequestAuthor,
	r.RequestDate as RequestDate
from dbo.tProject p
	inner join dbo.vRequest r on p.Id = r.RequestProjectId
where p.Id <> 0;
GO