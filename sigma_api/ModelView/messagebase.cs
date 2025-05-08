using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.Connection;

namespace sigma_api.ModelView
{
    public class messagebase
    {
        public bool success { get; set; }
        public int code { get; set; }
        public string message { get; set; } = null;
    }
}
