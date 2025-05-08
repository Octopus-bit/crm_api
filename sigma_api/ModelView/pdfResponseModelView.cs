using sigma_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.ModelView
{
    public class pdfResponseModelView : messagebase
    {
        public pdfModel url { get; set; }
        public pdfResponseModelView(pdfModel response)
        {
            success = true;
            code = 200;
            url = response;
        }
        public pdfResponseModelView(string error)
        {
            success = false;
            code = 0;
            message = error;
        }
    }
}
