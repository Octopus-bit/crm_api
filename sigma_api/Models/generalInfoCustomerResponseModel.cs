using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class generalInfoCustomerResponseModel // اطلاعات کاربر بر اساس شماره موبایل
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Birthdate { get; set; }
        public string MeliCode { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public int SalesExpert { get; set; }
        public string SalesExpert_FullName { get; set; }
        public datas[] Data { get; set; }

        public class datas
        {
            
        }
    }
}
