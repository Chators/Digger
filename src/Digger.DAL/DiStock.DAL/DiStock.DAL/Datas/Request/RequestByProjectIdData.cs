using System;
using System.Collections.Generic;
using System.Text;

namespace DiStock.DAL.Datas.Request
{
    public class RequestByProjectIdData
    {
        public int Id { get; set; }

        public string DataEntity { get; set; }

        public string UidNode { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }
    }
}
