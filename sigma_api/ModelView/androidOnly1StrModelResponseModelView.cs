using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;
namespace sigma_api.ModelView
{
    public class androidOnly1StrModelResponseModelView : messagebase
    {
        public crmOnly1StrModel results { get; set; }
        public androidOnly1StrModelResponseModelView(crmOnly1StrModel response)
        {
            success = true;
            code = ErrorCode.Created.GetHashCode();
            results = response;
        }
        public androidOnly1StrModelResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
