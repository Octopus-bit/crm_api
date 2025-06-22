using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatConversationInfoResponseModel // دریافت اطلاعات بازگشتی گفتگو
    {
        public int id { get; set; } // شناسه چت
        public int sender { get; set; } // ارسال کننده
        public int receiver { get; set; } // دریافت کننده
        public string lastMessage { get; set; } // اخرین پیام
        public int unReadCount { get; set; } // تعداد خوانده نشده ها
        public DateTime? atCreate { get; set; } // زمان ایجاد گفتگو
        public DateTime? atUpdate { get; set; } // زمان ابدیت گفتگو
        public bool status { get; set; } // 1 = open / 0 = close (وضعیت گفتگو)
    }
}
