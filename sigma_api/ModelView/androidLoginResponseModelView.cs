using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.ModelView;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class androidLoginResponseModelView : messagebase
    {
        public androidLoginResponseModel results { get; set; }
        public androidLoginResponseModelView(androidLoginResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public androidLoginResponseModelView(int errorCode,string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
