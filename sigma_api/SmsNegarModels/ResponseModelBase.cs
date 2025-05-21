using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseModelBase
    {
        public ResponseModelBase()
        {
            Status = 0;
        }
        /// <summary>
        /// متن خطایی که رخ داده
        /// </summary>
        public string Messege { get; set; }
        /// <summary>
        /// کد خطا در صورتی که عدد بالای صفر باشد یعنی خطایی رخ داده
        /// </summary>
        public int Status { get; set; }
    }
}