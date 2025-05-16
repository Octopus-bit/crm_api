using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class androidCallBatchRequestModelView
    {
        public string guid { get; set; }
        public string mobile { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public int state { get; set; }
        public int type { get; set; }
        public string duration { get; set; }
        public data Description { get; set; }
        public class data
        {
            public string commands { get; set; }
            public string alarm_date { get; set; }
            public string alarm_time { get; set; }
            public string alarm_desc { get; set; }
            public string location { get; set; }
        }
    }
}
