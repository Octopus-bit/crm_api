using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class androidCallResponseModelView : messagebase
    {
        public androidCallResponseModel resulte { get; set; }
        public androidCallResponseModelView(androidCallResponseModel response)
        {
            success = true;
            code = ErrorCode.Created.GetHashCode();
            resulte = response;
        }
        public androidCallResponseModelView(int errorCode,string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
