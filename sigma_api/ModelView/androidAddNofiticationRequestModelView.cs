using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.ModelView
{
    public class androidAddNofiticationRequestModelView
    {
        public int CSTID { get; set; }
        public string alarm_date { get; set; }
        public string alarm_time { get; set; }
        public string alarm_desc { get; set; }
    }
}
