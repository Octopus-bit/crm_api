using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmCycleRefferingResponseModel
    {
        public DateTime? SendDate { get; set; } // تاریخ ثبت
        public string Sender { get; set; } // ارجاع دهنده
        public string Reciever { get; set; } // ارجاع گیرنده
        public string Subject { get; set; } // موضوع
        public string Summary { get; set; } // خلاصه توضیحات ارجاع
        public DateTime ShowDate { get; set; } // زمان مشاهده
        public string Approver { get; set; } // تایید کننده
        public DateTime? ConfirmDate { get; set; } // زمان تایید
        public bool Valid_id { get; set; } // شناسه وضعیت اعتبار
    }
}
