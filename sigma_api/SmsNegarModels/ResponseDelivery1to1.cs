using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseDelivery1to1 : ResponseModelBase
    {
        public ResponseDelivery1to1()
        {
           // ReceiveModel = new List<ResponseReceiveModel>();
        }


        public DeliveryStatus[] DeliveryStatus { get; set; }

    }

   
}