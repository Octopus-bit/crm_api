using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmCityResponseModelView : messagebase
    {
        public object results { get; set; }
        public crmCityResponseModelView(ListResponse response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }

        public crmCityResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
