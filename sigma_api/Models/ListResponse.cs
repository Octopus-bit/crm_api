using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class ListResponse
    {
        public int Pages { get; set; }
        public int Page_row { get; set; }
        public object results { get; set; }
    }
}
