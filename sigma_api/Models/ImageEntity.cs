using System;

namespace sigma_api.Models
{
    public class ImageEntity
    {
        public int Id { get; set; }              // کلید اصلی
        public string FileName { get; set; }     // نام فایل
        public string ContentType { get; set; }  // نوع فایل (مثلاً image/png)
        public byte[] Data { get; set; }
        public string Description { get; set; }     // توضیحات
        
    }
}
