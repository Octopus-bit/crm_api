using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmLoginResponseModelView : messagebase
    {
        public crmLoginResponseModel resulte { get; set; }
        public crmLoginResponseModelView(crmLoginResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            resulte = response;
        }
        public crmLoginResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
