using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class Delivery1to1Model
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
        public long[] Ids { get; set; }
    }
}