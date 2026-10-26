using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmQuotationsResponseModel // اطلاعات یک پیش فاکتور
    {
        public int FormID { get; set; } // شناسه خرید
        public bool Pm { get; set; } // پیام دارد یا نه
        public string CustomersName { get; set; } // مشتری
        public int Cstid { get; set; } // شناسه مشتری
        public string StatePay { get; set; } // وضعیت خرید
        public bool TypeRequest { get; set; }//نوع درخواست
        public string dateForm { get; set; } // تاریخ خرید
        public string NumberForm { get; set; } // شماره خرید
        public string TypeSale { get; set; } // نوع فروش - نقدری / اعتباری / نقدی اعتباری
        public decimal Price { get; set; } // مبلغ کل
        public decimal Discount { get; set; } // تخفیف
        public decimal Tax { get; set; } // مالیات
        public decimal NetPrice { get; set; } // مبلغ خالص
        public int TypeForm { get; set; } // ثبت شده یا تایید شده
        public bool Portal { get; set; } // شناسه پورتال/سی ارم
       // public Cycle CycleStatus { get; set; }
        public Detail[] Details { get; set; }
        //public class Cycle // چرخه پیش فاکتور
        //{
        //    public int? RB_id { get; set; }
        //    public int? RB_status { get; set; }
        //    public int? QU_id { get; set; }
        //    public int? QU_status { get; set; }
        //    public int? INV_id { get; set; }
        //    public int? INV_status { get; set; }
        //    public int? LD_id { get; set; }
        //    public int? LD_status { get; set; }
        //}
        public class Detail // اطلاعات کالای پیش فاکتور
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
