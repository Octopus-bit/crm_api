using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json;
namespace sigma_api.Classes
{
    public enum ErrorCode : int
    {
        /// <summary>
        /// درخواست با موفقیت ارسال شد
        /// </summary>
        Send = 200,
        /// <summary>
        /// عملیات با موفقیت انجام شد
        /// </summary>
        Created = 201,
        /// <summary>
        /// عملیات با موفقیت انجام نشد
        /// </summary>
        Conflict = 202,
        /// <summary>
        /// داده های ارسال شده ناقص میباشد
        /// </summary>
        BadRequest = 203,
        /// <summary>
        /// داده تکراری ارسال شده است
        /// </summary>
        Duplicate = 204,
        /// <summary>
        /// عدم احراز هویت
        /// </summary>
        Authentication = 100,
        /// <summary>
        /// نام کاربری یا کلمه عبور نادرست
        /// </summary>
        InvalidUser = 101,
        /// <summary>
        /// عدم دسترسی
        /// </summary>
        NoAccess = 102,
        /// <summary>
        /// عدم دسترسی های خاص
        /// </summary>
        SpecialAccess = 103,
        /// <summary>
        /// خطا در پایگاه داده
        /// </summary>
        DBO_Error = 300,
        /// <summary>
        /// پایگاه داده در دسترس نیست
        /// </summary>
        DBO_Unavailable = 301
    }

    public enum TypeToken : int
    {
        /// <summary>
        /// توکن کاربران crm
        /// </summary>
        user = 0,
        /// <summary>
        /// توکن مشتریان crm
        /// </summary>
        customer = 1
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) : base(JsonConvert.SerializeObject(obj.ToString()), Encoding.UTF8, "application/json") { }
    }
}
