using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sigma_api.Connection;

namespace sigma_api.ModelView
{
    public class messagebase // ساختار json وب سرویس
    {

        // تابع ایجاد پیغام دلخواه
        public static string messages(bool status,int errorCode,string errorMessage)
        {
            return JsonConvert.SerializeObject(new messagebase()
            {
                success = status,
                code = errorCode,
                message = errorMessage
            });
        }
        public bool success { get; set; } // وضعیت درخواست 
        public int code { get; set; } // (کد های برگشتی (شخصی سازی شده
        public string message { get; set; } = null; // پیغام 
    }
}
