using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseUserInfo : ResponseModelBase
    {
        public ResponseUserInfo()
        {
         
        }
        public string[] Result { get; set; }

        /// <summary>
        /// شماره ارسالی کاربر
        /// </summary>
        public string SendrNumber { get; set; }
        /// <summary>
        /// شارژ
        /// </summary>
        public string Charge { get; set; }
        /// <summary>
        /// تعداد پیامک های ارسال شده
        /// </summary>
        public string NumberOfSms { get; set; }

        /// <summary>
        /// آخرین زمان ورود به نرم افزار
        /// </summary>
        public string LastTimeLogin { get; set; }
        /// <summary>
        /// مبلغ پیامک فارسی
        /// </summary>
        public string FarsiPrice { get; set; }

        /// <summary>
        /// مبلغ پیامک انگلیسی
        /// </summary>
        public int EnglishPrice { get; set; }
    }

   
}