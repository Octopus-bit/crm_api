using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    /// <summary>
    /// پیامک های دریافتی
    /// </summary>
    public class ReceiveModel
    {

        public string UserName { get; set; }

        public string Password { get; set; }


        public bool IsAllMessage { get; set; }

        public string DomainName { get; set; }

        public string FromNumber { get; set; }


        public string FromDate { get; set; }

        public string ToDate { get; set; }
      
    }
}