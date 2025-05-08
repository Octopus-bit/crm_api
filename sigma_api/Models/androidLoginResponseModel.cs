using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class androidLoginResponseModel
    {
        public int id { get; set; }
        public string fullname { get; set; }
        public data access { get; set; }
        public class data
        {
            public bool SoundRecord { get; set; }
            public bool GpsLocation { get; set; }
        }
    }
}
