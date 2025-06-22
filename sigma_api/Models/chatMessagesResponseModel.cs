using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatMessagesResponseModel // اطلاعات یک پیام
    {
        public int id { get; set; } // شناسه پیام
        public int? Sender { get; set; } // ارسال کننده
        public int? Receiver { get; set; } // دریافت کننده
        public string Text { get; set; } // متن پیام
        public DateTime? atCreate { get; set; } // زمان ارسال
        public DateTime? atSeen { get; set; } // زمان دیده شدن پیام
        public bool IsRead { get; set; } // ایا خوانده شده است ؟
        public bool HasFile { get; set; } // از نوع فایل میباشد ؟
        public object File { get; set; } // اطلاعات فایل ولی اینجا ارسال نمیشه
        public string FileName { get; set; } // نام فایل
        public int ReplyTo { get; set; } // پاسخ داده شده به شناسه پیام ؟
    }
}
