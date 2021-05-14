using System;
using System.Collections.Generic;
using System.Text;

namespace DiStock.DAL.Datas.Project
{
    public class ProjectUserIdData
    {
        public int Id { get; set; }

        public string AccessRight { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte IsPublic { get; set; }

        public DateTime Date { get; set; }
    }
}
