using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class androidActivityRequestAllModelView
    {
        public int AlarmId { get; set; }
        public int Userid { get; set; }
        public DateTime DateAlarm { get; set; }
        public string AlarmTime { get; set; }
        public string DescAlarm { get; set; }
        public string DescAlarmDetail { get; set; }
        public DateTime DateExpired { get; set; }

    }
}
