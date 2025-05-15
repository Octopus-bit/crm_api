using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmRegistryCodeResponseModelView : messagebase
    {
        public crmLoginResponseModel results { get; set; }
        public crmRegistryCodeResponseModelView(bool status,int Code, string Message = null,crmLoginResponseModel response = null)
        {
            success = status;
            code = Code;
            message = Message;
            results = response;
        }
    }
}
