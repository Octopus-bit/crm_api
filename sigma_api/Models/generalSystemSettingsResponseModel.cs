using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class generalSystemSettingsResponseModel
    {
        public string Company { get; set; }
        public string Logo { get; set; }
        public string Version { get; set; }
        public int ExpirationQuotations { get; set; }
        public bool Column_Unit2 { get; set; }

        public clsSidebar Sidebar { get; set; }
        public class clsSidebar
        {
            public int UnReadMessages { get; set; }
        }
    }
}
