using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class crmListActivityDetaillResponseModel
    {
        public int AlarmId { get; set; }
        public string ShowState { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public DateTime DateAlarm { get; set; }
        public string TimeAlarm { get; set; }
        public string DescAlarm { get; set; }
        public string DescAlarmDetail { get; set; }
        public DateTime? DateExpired { get; set; }
        public DateTime? Showdate { get; set; }
        public string ShowdateTime { get; set; }
        public DateTime? ChangedateState { get; set; }
    }
}
