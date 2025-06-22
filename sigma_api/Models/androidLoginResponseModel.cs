using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class androidLoginResponseModel
    {
        public string id { get; set; } // شناسه = توکن 
        public string fullname { get; set; } // نام کامل کاربر
        public data access { get; set; } // دسترسی های کاربر
        public class data
        {
            public bool SoundRecord { get; set; } // ثبت وضیحات تماس
            public bool GpsLocation { get; set; } // ثبت لوکیشن
        }
    }
}
