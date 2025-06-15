using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmDashboardResponseModel
    {
        public List<clsSales> SalesChart { get; set; }
        public List<clsSales> ComparisonSalesChart { get; set; }
        public clsStats Stats { get; set; }
        public clsSidebar Sidebar { get; set; }
        public class clsStats
        {
            public clsStatsValues invoice { get; set; }
            public clsStatsValues quotation { get; set; }
        }


        public class clsSales
        {
            public string date { get; set; }
            public decimal value { get; set; }
        }

        public class clsStatsValues
        {
            public int total { get; set; }
            public int approved { get; set; }
            public int cancelled { get; set; }
            public int pending { get; set; }
        }
        public class clsSidebar
        {
            public int UnReadMessages { get; set; }
        }
    }
}
