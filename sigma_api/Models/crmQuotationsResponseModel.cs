using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmQuotationsResponseModel
    {
        public int QU_id { get; set; } // شناسه پیش فاکتور
        public int QU_no { get; set; } // شماره پیش فاکتور
        public int? RB_no { get; set; } // شماره درخواست خرید
        public string QU_date { get; set; }
        //public string CycleFormDetailState { get; set; }
        public string Customer { get; set; } // نام مشتری
        public string Supplier { get; set; } // کارشناس فروش
        public string TypeSale { get; set; } // نوع فروش
        public string TypePayment { get; set; } // نوع پرداخت - روش پرداخت
        public decimal NetPrice { get; set; } // فی خالص
        public string City { get; set; } // شهر مقصد
        public decimal value { get; set; } // تعداد کل
        public decimal valueSecondary { get; set; } // تعداد کل ثانویه
        public string SalesExpert { get; set; }//کارشناس فروش
        public string Status { get; set; } // وضعیت
        public int StatusId { get; set; } //شناسه وضعیت 
        public int TypeSaleId { get; set; } // شناسه نوع فروش
        public int DLCode { get; set; } // کد تفصیلی
        public int SalesExpertId { get; set; } // شناسه کارشناس فروش
        public int CustomerId { get; set; } // شناسه مشتری 
        public string Tell { get; set; } // شماره تماس مشتری
        public Detail[] Details { get; set; }
        public class Detail
        {

        }
    }
}
