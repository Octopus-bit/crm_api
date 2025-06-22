using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class androidCallResponseModel // اطلاعات برگشتی پس از ثبت تماس تکی

    {
        public int id { get; set; }
        public string CustomersName { get; set; }
        public data access { get; set; }
        public class data
        {
            public bool SoundRecord { get; set; }
            public bool GpsLocation { get; set; }
        }
    }
}
