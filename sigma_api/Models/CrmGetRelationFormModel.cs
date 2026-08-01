using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class CrmGetRelationFormModel
    {
        public int ID { get; set; } // شناسه خرید
        public string DateForm { get; set; } // پیام دارد یا نه
        public string Number { get; set; } // شماره
        public string CycleFormDetailState { get; set; } // وضعیت چرخه
        public decimal valueform { get; set; } // وضعیت خرید
        public decimal NetPriceform { get; set; }//نوع درخواست
        public string SalesExpertform { get; set; } // تاریخ خرید
        public string SalesExpertId { get; set; } // ای دی کارشناس
    }
}
