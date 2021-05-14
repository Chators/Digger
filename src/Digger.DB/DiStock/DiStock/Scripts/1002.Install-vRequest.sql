create view [dbo].[vRequest]
	as
select
	r.Id as RequestId,
	r.FktProject as RequestProjectId,
	r.DataEntity as RequestDataEntity,
	r.UidNode as RequestUidNode,
	r.Author as RequestAuthor,
	r.Date as RequestDate,
	ssr.StatusRequest as RequestStatus
from dbo.tRequest r
	inner join dbo.tStaticStatusRequest ssr on r.FktStaticStatusRequest = ssr.Id
where r.Id <> 0;
GO
