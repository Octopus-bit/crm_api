using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatMessagesResponseModel
    {
        public int id { get; set; }
        public int? Sender { get; set; }
        public int? Receiver { get; set; }
        public string Text { get; set; }
        public DateTime? atCreate { get; set; }
        public DateTime? atSeen { get; set; }
        public bool IsRead { get; set; }
        public bool HasFile { get; set; }
        public object File { get; set; }
        public string FileName { get; set; }
        public int ReplyTo { get; set; }
    }
}
