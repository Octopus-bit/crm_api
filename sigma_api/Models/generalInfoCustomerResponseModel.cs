using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class generalInfoCustomerResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Birthdate { get; set; }
        public string MeliCode { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public datas[] Data { get; set; }

        public class datas
        {
            
        }
    }
}
