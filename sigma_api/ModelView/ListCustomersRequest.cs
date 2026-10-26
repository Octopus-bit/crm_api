using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class ListCustomersRequest
    {
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
        public string? SearchType { get; set; }
        public string? SearchName { get; set; }
        public string? SearchPhone { get; set; }
    }
}
