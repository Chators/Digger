create view [dbo].[vSoftware]
	as
select
	s.Id as SoftwareId,
	s.Name as SoftwareName,
	s.Description as SoftwareDescription,
	s.AverageExecutionTime as SoftwareAverageExecutionTime,
	rm.ResearchModuleId as ResearchModuleId,
	rm.ResearchModuleName as ResearchModuleName,
	rm.ResearchModuleDescription as ResearchModuleDescription,
	rm.ResearchModuleAverageExecutionTime as ResearchModuleAverageExecutionTime,
	rm.ResearchModuleLevelFootprint as ResearchModuleLevelFootprint,
	rm.ResearchModuleTypeEntity as ResearchModuleTypeEntity
from dbo.tSoftware s
	inner join dbo.vResearchModule rm on s.Id = rm.ResearchModuleFktSoftware
where s.Id <> 0;
GO