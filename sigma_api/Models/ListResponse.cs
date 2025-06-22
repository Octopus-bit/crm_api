using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class ListResponse // این مدل برای ایجاد خروجی اطلاعات از نوع لیست میباشد
    {
        public int Pages { get; set; } // تعداد کل صفهات
        public int Page_row { get; set; } // صفحه فعلی
        public object List { get; set; } // یک ابجکت از نوع لیست که هر نوع از خروجی های ریسپانس رو میتونید داخلش جای بدین
    }
}
