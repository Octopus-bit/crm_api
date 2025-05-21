using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class ResponseCodes: ResponseModelBase
    {
        public string[] ReturnCodes { get; set; }
    }
}