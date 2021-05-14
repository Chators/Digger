create view [dbo].[vResearchModule]
	as
select
	rm.Id as ResearchModuleId,
	rm.FktSoftware as ResearchModuleFktSoftware,
	rm.Name as ResearchModuleName,
	rm.Description as ResearchModuleDescription,
	rm.AverageExecutionTime as ResearchModuleAverageExecutionTime,
	sf.LevelFootprint as ResearchModuleLevelFootprint,
	se.TypeEntity as ResearchModuleTypeEntity
from dbo.tResearchModule rm
	inner join dbo.tStaticFootprint sf on rm.FktStaticFootprint = sf.Id
	inner join dbo.tStaticEntity se on rm.FktStaticEntity = se.Id
where rm.Id <> 0;
GO