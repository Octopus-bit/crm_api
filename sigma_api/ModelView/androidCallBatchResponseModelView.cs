using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class androidCallBatchResponseModelView : messagebase
    {
        public List<androidCallBatchResponseModel> results { get; set; }
        public androidCallBatchResponseModelView(List<androidCallBatchResponseModel> response)
        {
            success = true;
            code = ErrorCode.Created.GetHashCode();
            results = response;
        }
        public androidCallBatchResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
