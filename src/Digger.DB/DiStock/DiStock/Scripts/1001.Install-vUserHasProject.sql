create view [dbo].[vUserHasProject]
	as
select
	uhp.FktUser as UserId,
	uhp.FktProject as ProjectId,
	spar.AccessRight as AccessRight
from dbo.tUserHasProject uhp
	inner join dbo.tStaticProjectAccessRight spar on uhp.FktStaticProjectAccessRight = spar.Id
GO