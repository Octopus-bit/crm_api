using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class CrmGetRelationFormDetailModel
    {
        public string Senddate { get; set; } // تاریخ ارسال
        public string TypeReffer { get; set; } // نوع ارجاع
        public string Sender { get; set; } // فرستنده
        public string Subject { get; set; } // موضوع
        public string typeChart { get; set; }//وضعیت چرخه
        public string Description { get; set; } // شرح ارجاع
        public string Receiver { get; set; } // گیرنده
        public string Showdate { get; set; } // تاریخ مشاهده
        public string Confirmer { get; set; } // تایید کننده
        public string Confirmdate { get; set; } // تاریخ تایید
    }
}
