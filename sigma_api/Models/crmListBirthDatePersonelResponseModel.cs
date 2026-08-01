using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace sigma_api.Models
{
    public class crmListBirthDatePersonelResponseModel
    {
        public int PersonelId { get; set; }
        public DateTime birthdate { get; set; }
        public string birthdateShamsi { get; set; }
        public string PersonelIdName { get; set; }
    }
}
