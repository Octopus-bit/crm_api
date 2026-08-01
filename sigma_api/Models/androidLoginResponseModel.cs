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
        public string AdditionItem1 { get; set; } // فیلد اضافه1
        public string AdditionItem2 { get; set; } // فیلد اضافه2
        public data access { get; set; } // دسترسی های کاربر
        public class data
        {
            public bool SoundRecord { get; set; } // ثبت وضیحات تماس
            public bool GpsLocation { get; set; } // ثبت لوکیشن
            public bool Country { get; set; } // ثبت لوکیشن
        }
    }
}
