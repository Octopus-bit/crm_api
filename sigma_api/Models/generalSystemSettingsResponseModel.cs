using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class generalSystemSettingsResponseModel // اتطلاعات سیستم و تنظیمات جدول propertise
    {
        public string Company { get; set; } // نام شرکت
        public string Logo { get; set; } // لوگو نرم افزار
        public string Version { get; set; } // ورژن api
        public int ExpirationQuotations { get; set; } // تعداد روز انقضای پیش فاکتور
        public bool Column_Unit2 { get; set; } // ایا ستون واحد دوم نمایش داده شود ؟
        public int DefualtTasfiePortal { get; set; } // پیش فرض نحوه تسویه
        public int insertbuyPortal { get; set; } // نوع ثبت پورتال پیش فاکتور یا درخواست خرید
        public bool ShowPictureItemPortal { get; set; } // نمایش یا عدم نمایش تصویر کالا در پورتال
        public string ItemAddition1 { get; set; } // فیلد اضافه1 کالا
        public string ItemAddition2 { get; set; } // فیلد اضافه2 کالا
        public bool ShowPortalItemAdd1 { get; set; }//نمایش فیلد اضافه 1 به ای انبار 
        public clsSidebar Sidebar { get; set; } // سایدبار
        public class clsSidebar
        {
            public int UnReadMessages { get; set; } // تعداد پیام خوانده نشده کاربر فعلی
        }
    }
}
