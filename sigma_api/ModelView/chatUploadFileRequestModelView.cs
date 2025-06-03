using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace sigma_api.ModelView
{
    public class chatUploadFileRequestModelView
    {
        public int MessageId { get; set; }
        public IFormFile File { get; set; }
        public string FileName { get; set; }
    }
}
