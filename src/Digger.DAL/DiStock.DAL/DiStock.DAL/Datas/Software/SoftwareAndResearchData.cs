using System;
using System.Collections.Generic;
using System.Text;

namespace DiStock.DAL.Datas
{
    public class SoftwareAndResearchData
    {
        public SoftwareData Software { get; set; }

        public List<ResearchModuleData> ResearchModule { get; set; }

        public SoftwareAndResearchData(SoftwareData software, List<ResearchModuleData> researchModule)
        {
            Software = software;
            ResearchModule = researchModule;
        }
    }
}
