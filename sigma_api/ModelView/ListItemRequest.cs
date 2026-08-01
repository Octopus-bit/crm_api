using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class ListItemRequest
    {
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
        public string? SearchName { get; set; }
        public string? SearchType { get; set; }
        public int? SearchGroupItem { get; set; }
        public int? SearchItemAddition1 { get; set; }
        public int? SearchItemAddition2 { get; set; }
    }
}
