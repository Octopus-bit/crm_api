using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseDelivery : ResponseModelBase
    {
        public ResponseDelivery()
        {
           // ReceiveModel = new List<ResponseReceiveModel>();
        }


        public DeliveryStatus[] DeliveryStatus { get; set; }

    }

   
}