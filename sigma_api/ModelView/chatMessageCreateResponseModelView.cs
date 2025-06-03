using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;

namespace sigma_api.ModelView
{
    public class chatMessageCreateResponseModelView : messagebase
    {
        public object results { get; set; }

        private class CreatedMessage
        {
            public int message_id { get; set; }
        }
        public chatMessageCreateResponseModelView(int message_id)
        {
            success = true;
            code = ErrorCode.Created.GetHashCode();
            results = new CreatedMessage()
            {
                message_id = message_id,
            };
        }

        public chatMessageCreateResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }

    }
}
