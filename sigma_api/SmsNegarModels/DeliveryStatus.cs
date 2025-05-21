using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class DeliveryStatus
    {
     
        public DeliveryStatus()
        {
         
        }

      
        /// <summary>
        /// شناسه خطا
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// متن خطا
        /// </summary>
        public string Message { get; set; }
    }
}