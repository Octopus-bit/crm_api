using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class crmListAlarmResponseModel
    {
        public int SCC_id { get; set; }
        public DateTime AlarmDate { get; set; }
        public string Customers { get; set; }
        public int Customersid { get; set; }
        public string descriptions { get; set; }
    }
}
