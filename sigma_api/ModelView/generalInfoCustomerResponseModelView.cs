using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class generalInfoCustomerResponseModelView : messagebase
    {

        public generalInfoCustomerResponseModel results { get; set; }
        public generalInfoCustomerResponseModelView(generalInfoCustomerResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public generalInfoCustomerResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
