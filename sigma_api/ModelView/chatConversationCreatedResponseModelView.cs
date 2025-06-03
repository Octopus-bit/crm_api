using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Classes;
using sigma_api.Models;

namespace sigma_api.ModelView
{
    public class chatConversationCreatedResponseModelView : messagebase
    {
        public object results { get; set; }

        private class CreatedConversation
        {
            public int Con_id { get; set; }
        }
        public chatConversationCreatedResponseModelView(int ConversationId){
            success = true;
            code = ErrorCode.Created.GetHashCode();
            results = new CreatedConversation()
            {
                Con_id = ConversationId,
            };
        }

        public chatConversationCreatedResponseModelView(int errorCode, string errorMessage = null)
        {
            success = false;
            code = errorCode;
            message = errorMessage;
        }
    }
}
