using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatUserSResponseModel
    {
        public int id { get; set; } // شناسه کاربر چت که از جدول CHAT_Users استفاده میشه این شناسه پس از لاگین به api با تریگر به صورت اتوماتیک ایجاد میشود
        public string Name { get; set; } // نام کاربر
        public string phone { get; set; } // شماره تماس
        public string Image { get; set; }
    }
}
