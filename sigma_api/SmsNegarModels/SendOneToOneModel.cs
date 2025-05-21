using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sigma_api.SmsNegarModels
{
    public class SendOneToOneModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string DomainName { get; set; }

        public List<Message> Messages { get; set; }
    }

    public class Message
    {

        public string Smsbody { get; set; }
        public string SenderNumber { get; set; }
        public string Mobile { get; set; }
        public long Id { get; set; }
    }
}