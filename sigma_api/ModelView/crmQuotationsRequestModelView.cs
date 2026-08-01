using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Models;

namespace sigma_api.ModelView
{
       public class crmQuotationsRequestModelView
    {
        public DateTime Date { get; set; } // تاریخ
        public string Description { get; set; } // توضیحات
        public int Location { get; set; } // شهر محل تحویل
        public string Address { get; set; } // ادرس محل تحویل
        public int TypeSale { get; set; } // نوع فروش - نقدری / اعتباری / نقدی اعتباری
        public int SalesExpert { get; set; } // کارشناس فروش
        public Item[] Items { get; set; }
        public class Item
        {
            public int IS_id { get; set; } // شناسه کالا و انبار
            public double value { get; set; } // تعداد
            public double valueSecondary { get; set; } // تعداد دوم
            public decimal fi { get; set; } // فی
            public decimal Price { get; set; } // مبلغ کل
            public decimal NetPrice { get; set; } // مبلغ کل خالص
        }
    }
}
