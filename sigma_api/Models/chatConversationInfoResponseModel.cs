using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatConversationInfoResponseModel
    {
        public int id { get; set; }
        public int sender { get; set; }
        public int receiver { get; set; }
        public string lastMessage { get; set; }
        public int unReadCount { get; set; }
        public DateTime? atCreate { get; set; }
        public DateTime? atUpdate { get; set; }
        public bool status { get; set; }
    }
}
