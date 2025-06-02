using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class crmCycleRefferingResponseModelView : messagebase
    {
        public List<crmCycleRefferingResponseModel> results { get; set; }
        public crmCycleRefferingResponseModelView(List<crmCycleRefferingResponseModel> response = null)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public crmCycleRefferingResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
