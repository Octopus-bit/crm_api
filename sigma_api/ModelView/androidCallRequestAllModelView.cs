using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class androidCallRequestAllModelView
    {
        public string mobile { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public int state { get; set; }
        public int type { get; set; }
        public string duration{ get; set; }
        public int id { get; set; }

        public class androidUpdateCallRequestModelView
        {
            public string commands { get; set; }
            public string alarm_date { get; set; }
            public string alarm_time { get; set; }
            public string alarm_desc { get; set; }
            public string location { get; set; }
            public int call_id { get; set; }

        }
       
        public class androidUpdateCallLocationRequestModelView
        {
            public string location { get; set; }
        }
        
    }
}

//94.183.93.219:5488
