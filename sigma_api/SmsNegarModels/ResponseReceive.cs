using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseReceive: ResponseModelBase
    {
        public ResponseReceive()
        {
            ReceiveModel = new List<ResponseReceiveModel>();
        }


        public List<ResponseReceiveModel> ReceiveModel { get; set; }

    }

    public class ResponseReceiveModel
    {

        public string Mobile { get; set; }

        public string SenderNumber { get; set; }

        public string Text { get; set; }

        public string DateSmsF { get; set; }

        public string DateSmsM { get; set; }
        public long MessageId { get; set; }
    }
}