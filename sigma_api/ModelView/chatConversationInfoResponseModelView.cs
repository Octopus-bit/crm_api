using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class chatConversationInfoResponseModelView : messagebase
    {
        public object results { get; set; }
        public chatConversationInfoResponseModelView(List<chatConversationInfoResponseModel> response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public chatConversationInfoResponseModelView(chatConversationInfoResponseModel response)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }

        public chatConversationInfoResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
