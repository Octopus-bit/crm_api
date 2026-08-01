using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace sigma_api.ModelView
{
    public class UploadImagesRequest
    {
        public List<IFormFile> Images { get; set; }
        public string InputStr { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
