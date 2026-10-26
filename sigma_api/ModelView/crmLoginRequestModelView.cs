using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class crmLoginRequestModelView
    {
        public string username { get; set; }
        public string password { get; set; }
        public int Type { get; set; }
    }
}
