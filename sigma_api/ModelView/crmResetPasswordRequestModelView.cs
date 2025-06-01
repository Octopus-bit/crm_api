using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class crmResetPasswordRequestModelView
    {
        [Required]
        public string new_password { get; set; }
    }
}
