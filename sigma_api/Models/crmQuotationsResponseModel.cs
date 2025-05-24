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
            public int id { get; set; } // شناسه ایتم پیش فاکتور
            public string Code { get; set; } // کد کالا
            public string StockTitle { get; set; } // نام انبار
            public string Title { get; set; } // نام کالا
            public string SupplierId { get; set; } // شناسه تامین کننده 
            public string SupplierName { get; set; } // نام تامین کننده
            public string Unit1 { get; set; } // واحد اصلی
            public double value { get; set; } // تعداد
            public double valueSecondary { get; set; } // تعداد دوم
            public double fee { get; set; } // فی
            public double Price { get; set; } // مبلغ کل
            public double Discount { get; set; } // مبلغ تخفیف ردیف
            public double DistinctPercent { get; set; } //درصد تخفیف ردیف
            public double Tax { get; set; } // مالیات
            public double Duty { get; set; }//عوارض
            public double Addition { get; set; } // اضافات
            public double NetPrice { get; set; } // 
            public string Unit2 { get; set; } //  واحد دوم
            public string Nesabt { get; set; } // نسبت دارد یا ندارد
            public double NesbatValue { get; set; } // نسبت ولیو
            public double Item_tax { get; set; } // مالیات
            public double Item_duty { get; set; } // 
            public int Item_id { get; set; } // شناسه کالا
            public string StockId { get; set; } // شناسه انبار
            public string DistinctCustomersBrand { get; set; } // درصد تخفیف مشتری روی برند
            public string ItemStock_Id { get; set; } // شناسه کالا و انبار
            public string ItemBrand { get; set; } // نام برند

        }
    }
}
