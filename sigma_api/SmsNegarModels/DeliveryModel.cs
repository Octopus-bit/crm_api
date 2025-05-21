using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class DeliveryModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Domain { get; set; }
        public string Id { get; set; }
    }
}