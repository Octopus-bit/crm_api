using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmLoginResponseModel // اطلاعات کاربر لاگین شده
    {
        public string id { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MeliCode { get; set; }
        public string Email { get; set; }
        public string Birthdate { get; set; }
        public data access { get; set; }
        public class data
        {
            
        }
    }
}
