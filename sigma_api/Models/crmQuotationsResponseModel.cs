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
        public string Supplier { get; set; } // تامین کننده
        public string TypeSale { get; set; } // نوع فروش - نقدری / اعتباری / نقدی اعتباری
        public string TypePayment { get; set; } // نوع پرداخت - روش پرداخت
        public decimal NetPrice { get; set; } // مبلغ خالص
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
        public Cycle CycleStatus { get; set; }
        public Detail[] Details { get; set; }

        public class Cycle
        {
            public int? RB_status { get; set; }
            public int? QU_status { get; set; }
            public int? INV_status { get; set; }
        }
        public class Detail
        {
            public int id { get; set; } // شناسه ایتم پیش فاکتور
            public string Code { get; set; } // کد کالا
            public string StockTitle { get; set; } // نام انبار
            public string Title { get; set; } // نام کالا
            public int SupplierId { get; set; } // شناسه تامین کننده 
            public string SupplierName { get; set; } // نام تامین کننده
            public string Unit1 { get; set; } // واحد اصلی
            public double value { get; set; } // تعداد
            public double valueSecondary { get; set; } // تعداد دوم
            public decimal fee { get; set; } // فی
            public decimal Price { get; set; } // مبلغ کل
            public decimal Discount { get; set; } // مبلغ تخفیف ردیف
            public double DistinctPercent { get; set; } //درصد تخفیف ردیف
            public int Tax { get; set; } // مالیات
            public int Duty { get; set; }//عوارض
            public decimal Addition { get; set; } // اضافات
            public decimal NetPrice { get; set; } // 
            public string Unit2 { get; set; } //  واحد دوم
            public bool Nesabt { get; set; } // نسبت دارد یا ندارد
            public double NesbatValue { get; set; } // نسبت ولیو
            public int Item_tax { get; set; } // مالیات
            public int Item_duty { get; set; } // 
            public int Item_id { get; set; } // شناسه کالا
            public int StockId { get; set; } // شناسه انبار
            public double DistinctCustomersBrand { get; set; } // درصد تخفیف مشتری روی برند
            public int ItemStock_Id { get; set; } // شناسه کالا و انبار
            public string ItemBrand { get; set; } // نام برند

        }
    }
}
