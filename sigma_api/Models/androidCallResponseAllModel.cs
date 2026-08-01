using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class androidCallResponseAllModel // اطلاعات برگشتی پس از ثبت تماس تکی

    {
        public int id { get; set; }
        public string CustomersName { get; set; }
        public int LocalId { get; set; }
    }
}
