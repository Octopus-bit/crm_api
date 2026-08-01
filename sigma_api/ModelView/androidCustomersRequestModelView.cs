using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.ModelView
{
    public class androidCustomersRequestModelView
    {
        public bool TypeCustomers { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int IdCity { get; set; }
        public int IdOstan { get; set; }
        public int IdCountry { get; set; }
        public int ItemAdditonId1 { get; set; }
        public int ItemAdditonId2 { get; set; }
        public string mobile { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string BirthDate { get; set; }
        //public string location { get; set; }

    }
}
