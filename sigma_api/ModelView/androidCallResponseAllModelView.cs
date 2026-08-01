using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class androidCallResponseAllModelView : messagebase
    {
        public List<androidCallResponseAllModel> results { get; set; }
        public androidCallResponseAllModelView(List<androidCallResponseAllModel> response)
        {
            success = true;
            code = ErrorCode.Created.GetHashCode();
            results = response;
        }
        public androidCallResponseAllModelView(int errorCode,string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
