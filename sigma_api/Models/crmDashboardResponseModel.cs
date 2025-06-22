using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmDashboardResponseModel // اطلاعات داشبورد
    {
        public List<clsSales> SalesChart { get; set; }
        public List<clsSales> ComparisonSalesChart { get; set; }
        public clsStats Stats { get; set; } // این بخش برای باکث های فاکتور و پیش فاکتور هست

        public class clsStats
        {
            public clsStatsValues invoice { get; set; }
            public clsStatsValues quotation { get; set; }
        }


        public class clsSales // نمایش اطلاعات چارت تشکیل شده از بازه تاریخی و مقدار
        {
            public string date { get; set; }
            public decimal value { get; set; }
        }

        public class clsStatsValues // این کلاس برای نماش تعداد تاییدی ابطالی در انتضار و همه میباشد
        {
            public int total { get; set; }
            public int approved { get; set; }
            public int cancelled { get; set; }
            public int pending { get; set; }
        }

    }
}
