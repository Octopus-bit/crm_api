using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class crmListBirthDateResponseModel
    {
        public int Customersid { get; set; }
        public DateTime birthdate { get; set; }
        public string birthdateShamsi { get; set; }
        public string CustomersName { get; set; }
        public string Phone { get; set; }

    }
}
