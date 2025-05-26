using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class generalSystemSettingsResponseModelView : messagebase
    {
        public generalSystemSettingsResponseModel results { get; set; }
        public generalSystemSettingsResponseModelView(generalSystemSettingsResponseModel response = null)
        {
            success = true;
            code = ErrorCode.Send.GetHashCode();
            results = response;
        }
        public generalSystemSettingsResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
