using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatUserInfoResponseModel // دریافت اطلاعات کاربر داخل چت
    {
        public int id { get; set; } // شناسه کاربر چت که از جدول CHAT_Users استفاده میشه این شناسه پس از لاگین به api با تریگر به صورت اتوماتیک ایجاد میشود
        public string Name { get; set; } // نام کاربر
        public DateTime? lastseen  { get; set; }// اخرینزمان انلاین بودن
        public int unReadCount { get; set; } // تعداد پیام خوانده نشده
        public string email { get; set; } // ایمیل
        public string phone { get; set; } // شماره تماس
        public string? avatar { get; set; } // لینک پروفایل کاربر
        public string bio { get; set; } // بیو کاربر
        public int status { get; set; } // 0 = offline / 1 = online / 2 = away
        public string statusName { get; set; } // (وضعیت کاربر (افلاین ، انلاین ، نامشخص
    }
}
