using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class chatUserInfoResponseModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public DateTime? lastseen  { get; set; }
        public int unReadCount { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string? avatar { get; set; }
        public string bio { get; set; }
        public int status { get; set; }
        public string statusName { get; set; }
    }
}
