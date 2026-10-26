using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class generalGetCustomerResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Address { get; set; }
        public int city { get; set; }
        public string cityName { get; set; }
        public string CustomersCode { get; set; }
        public string Mobile { get; set; }
    }
}
