using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmQuotationsResponseModelView : messagebase
    {
        public object results { get; set; }

        public crmQuotationsResponseModelView(List<crmQuotationsResponseModel> response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public crmQuotationsResponseModelView(crmQuotationsResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }

        public crmQuotationsResponseModelView(int errorCode,string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
