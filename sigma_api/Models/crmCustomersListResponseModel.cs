using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmCustomersListResponseModel
    {
        public int cst_id { get; set; }
        public string CustomersName { get; set; }
        public string CustomersType { get; set; }
        public string CustomersCode { get; set; }
        public string Ostan { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
