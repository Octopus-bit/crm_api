using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class SendModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Smsbody { get; set; }
        public string[] Mobiles { get; set; }
        public string Id { get; set; }
        public string DomainName { get; set; }
        public string SenderNumber { get; set; }
    }
}