using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmItemListResponseModelView : messagebase
    {
        public object results { get; set; }
        public crmItemListResponseModelView(List<crmItemListResponseModel> response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public crmItemListResponseModelView(crmItemListResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public crmItemListResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
