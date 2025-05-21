using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseOneToOneCode : ResponseModelBase
    {
        /// <summary>
        /// کدهای ارسالی
        /// </summary>
        public long[] ReturnCodes { get; set; }
    }
}