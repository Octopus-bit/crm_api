using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.ModelView;
using sigma_api.Models;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Security;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Transactions;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Net.Http;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static sigma_api.ModelView.crmQuotationsRequestModelView;
using Microsoft.EntityFrameworkCore;
using iText.StyledXmlParser.Jsoup.Select;
using System.Windows.Forms;
using Microsoft.Graph.Models.Security;
using System.Text.RegularExpressions;
using System.Reflection;

namespace sigma_api.Controllers
{

    public class androidController : Controller
    {
        // کنترلر های اپلیکیشن اندروید
        cls_public cp = new cls_public();
        androidLoginResponseModelView response_login;
        androidCallResponseModelView response_call;
        androidCallResponseAllModelView response_call_all;
        androidOnly1StrModelResponseModelView response_Customer;
        androidCallBatchResponseModelView response_callBatch;
        crmCityResponseModelViewALL response_City;
        [HttpPost]
        [Route("api/[Controller]/login")] // درخواست ورود
        [AllowAnonymous]
        public string login(androidLoginRequestModelView request)
        {
            string username, password;
            // بررسی ورودی ها
            if (string.IsNullOrEmpty(request.username))
            {
                response_login =
                    new androidLoginResponseModelView(ErrorCode.BadRequest.GetHashCode(),
                        "نام کاربری خالی میباشد");
                return JsonConvert.SerializeObject(response_login);
            }
            if (string.IsNullOrEmpty(request.password))
            {
                response_login =
                    new androidLoginResponseModelView(ErrorCode.BadRequest.GetHashCode(),
                        "کلمه عبور خالی میباشد");
                return JsonConvert.SerializeObject(response_login);
            }
            // ----------------------------------
            username = request.username;
            password = coding.SigmaEncryptString(/*coding.SafeFarsiStr(*/request.password/*)*/); // کد کردن پسورد ارسال شده
            SqlParameter[] parameters =
            {
                new SqlParameter("username",username),
                new SqlParameter("password",password ?? (object)DBNull.Value),
                new SqlParameter("Type","0") // ( نوع یوزر 2 دسته دارد ( 0 = کاربر سیستمی / 1 = مشتری های سیستم
            };
            DataTable dt = cp.select_row("API_CheckUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                //Classes.Library.WriteErrorLog(" Error API_CheckUser " + cls_public.sqlError.message);

                response_login = new androidLoginResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {
                if (dt.Rows.Count == 1)
                {
                    //Classes.Library.WriteErrorLog(" Login OK ");

                    int user_Id = dt.Rows[0]["USR_id"].ToInt(); // ایدی کاربر
                    string user_Name = dt.Rows[0]["Name"].ToString(); // نام کاربر
                    bool Status = dt.Rows[0]["USR_type"].ToBoolean(); // وضعیت کاربر فعال یا غیر فعال
                    bool UserMobile = dt.Rows[0]["USR_mobile"].ToBoolean(); // ایا کاربر دسترسی به اپ موبایل دارد ؟
                    bool SoundRecord = dt.Rows[0]["USR_SoundRecord"].ToBoolean(); // دسترسی برای ثبت توضیحات پس از تماس
                    bool GpsLocation = dt.Rows[0]["USR_GpsLocation"].ToBoolean(); // ثبت لوکیشن بر قراری تماس
                    bool Country = dt.Rows[0]["Country"].ToBoolean(); // ثبت لوکیشن بر قراری تماس
                    if (Status == false)
                    {
                        response_login = new androidLoginResponseModelView(ErrorCode.NoAccess.GetHashCode(), "حساب کاربری غیرفعال میباشد");
                    }
                    else if (UserMobile == false)
                    {
                        response_login = new androidLoginResponseModelView(ErrorCode.SpecialAccess.GetHashCode(), "این حساب کاربری به اپلیکیشن موبایل دسترسی ندارد");
                    }
                    else
                    {
                        string token = TokenManagement.WriteToken(user_Id, TypeToken.user.GetHashCode());
                        if (TokenManagement.IsCreated)
                        {
                            string baseQuery = $"select TNZ_sharh from PRG_Propertise where TNZ_name='desc1customer' ";
                            DataTable AdditionItem1 = cp.select_command(baseQuery);
                            baseQuery = $"select TNZ_sharh from PRG_Propertise where TNZ_name='desc2customer' ";
                            DataTable AdditionItem2 = cp.select_command(baseQuery);

                            androidLoginResponseModel data = new androidLoginResponseModel();
                            data.id = token;
                            data.fullname = user_Name;
                            if (AdditionItem1 != null)
                            {
                                if (AdditionItem1.Rows.Count > 0)
                                {
                                    data.AdditionItem1 = AdditionItem1.Rows[0][0].ToString();
                                    //Classes.Library.WriteErrorLog(" AdditionItem1" + AdditionItem1.Rows[0][0].ToString());
                                }
                                else
                                    data.AdditionItem1 = "";
                            }
                            else
                                data.AdditionItem1 = "";
                            if (AdditionItem2 != null)
                            {
                                if (AdditionItem2.Rows.Count > 0)
                                {
                                    data.AdditionItem2 = AdditionItem2.Rows[0][0].ToString();
                                    //Classes.Library.WriteErrorLog(" AdditionItem2" + AdditionItem2.Rows[0][0].ToString());
                                }
                                else
                                    data.AdditionItem2 = "";
                            }
                            else
                                data.AdditionItem2 = "";

                            data.access = new androidLoginResponseModel.data();
                            data.access.SoundRecord = SoundRecord.ToBoolean();
                            data.access.GpsLocation = GpsLocation.ToBoolean();
                            data.access.Country = Country.ToBoolean();
                            response_login = new androidLoginResponseModelView(data);
                            HttpContext.Response.Headers.Add("Token", token);
                        }
                        else
                        {
                           // Classes.Library.WriteErrorLog(" احراز هویت با مشکل مواجه شد"  );

                            response_login =
                                new androidLoginResponseModelView(ErrorCode.Authentication.GetHashCode(),
                                    "احراز هویت با مشکل مواجه شد");
                        }
                    }
                }
                else
                {
                    //Classes.Library.WriteErrorLog(" نام کاربری یا کلمه عبور اشتباه میباشد");

                    response_login = new androidLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }

            return JsonConvert.SerializeObject(response_login);
        }

        [HttpPost]
        [Route("api/[Controller]/UploadImages")]
        [AllowAnonymous]
        public async Task<string> UploadImages([FromForm] UploadImagesRequest request)
        {
            string location = request.Location;
            string InputStr = request.InputStr;
            string Description = request.Description;
            if (request.Images == null || !request.Images.Any())
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "هیچ تصویری ارسال نشده است");
                //response_login = new androidLoginResponseModelView(
                //    ErrorCode.BadRequest.GetHashCode(),
                //    "هیچ تصویری ارسال نشده است");
                //return JsonConvert.SerializeObject(response_login);
            }
            SqlParameter[] parameters =
            {
                //new SqlParameter("CustomersName",CustomersName),
                new SqlParameter("Mobile",InputStr),
            };
            DataTable dtcst = cp.select_row("API_SelectCustomersFromMobile", parameters);
            if (dtcst == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "مشتری با شماره مورد نظر یافت نشد");
            }
            else
            {
                if (dtcst.Rows.Count == 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "مشتری با شماره مورد نظر یافت نشد");
                }
            }
            DataTable DtValidFiledatabase = new DataTable();
            DtValidFiledatabase.Columns.Add("cst", typeof(int));
            DtValidFiledatabase.Columns.Add("dest", typeof(string));
            DtValidFiledatabase.Columns.Add("route", typeof(string));
            DtValidFiledatabase.Columns.Add("file", typeof(byte[]));
            DtValidFiledatabase.Columns.Add("tp", typeof(string));

            //DataTable dtImage = new DataTable();
            //dtImage.Columns.Add("FileName", typeof(string));
            //dtImage.Columns.Add("ContentType", typeof(string));
            //dtImage.Columns.Add("Data", typeof(byte[])); 
            foreach (var image in request.Images)
            {
                if (image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await image.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();

                        var entity = new ImageEntity
                        {
                            Id = dtcst.Rows[0][0].ToInt(),// InputStr.ToInt(),
                            FileName = image.FileName,
                            ContentType = image.ContentType,
                            Data = fileBytes,
                            Description = Description
                        };//
                        if (string.IsNullOrEmpty(Description))
                            Description = "تصویر دریافتی از نرم افزار موبایل";
                        DtValidFiledatabase.Rows.Add(entity.Id, Description, entity.FileName, entity.Data, "10");
                    }
                }
            }
            string id = cp.insert_DatabTable("SPandroid_InsertImageCustomers", DtValidFiledatabase);
            if (cls_public.sqlError.success == false)
            {
                response_Customer = new androidOnly1StrModelResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(),
                    cls_public.sqlError.message);
            }
            else
            {
                crmOnly1StrModel data = new crmOnly1StrModel();
                data.Resualt = id.ToString();
                response_Customer = new androidOnly1StrModelResponseModelView(data);
            }
            return JsonConvert.SerializeObject(response_Customer);
        }
        private readonly HttpClient _http = new HttpClient();
        private async Task<string> ReverseGeocodeAsync(double lat, double lon)
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("SigmaLocation/1.0 (SigmaGroup1403@Gmail.com)");

            string url =
                $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}&accept-language=fa";

            using var resp = await _http.GetAsync(url);

            resp.EnsureSuccessStatusCode();

            string json = await resp.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            // display_name کل آدرس یکجا است
            if (doc.RootElement.TryGetProperty("display_name", out var display))
            {
                return display.GetString();
            }

            return null;
        }

        private string ResolveBatchLocationAddress(string location)
        {
            if (string.IsNullOrEmpty(location))
                return "";

            try
            {
                string[] strloc = location.Split(',');
                if (strloc.Length < 2)
                    return "";

                string lat = strloc[0];
                string lon = strloc[1];
                _http.DefaultRequestHeaders.UserAgent.ParseAdd("SigmaLocation/1.0 (SigmaGroup1403@Gmail.com)");
                string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}";
                var response = _http.GetStringAsync(url).GetAwaiter().GetResult();
                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;
                if (root.TryGetProperty("address", out var addressLoc))
                {
                    string road = addressLoc.TryGetProperty("road", out var r) ? r.GetString() : "";
                    string neighbourhood = addressLoc.TryGetProperty("neighbourhood", out var n) ? n.GetString() : "";
                    string city = addressLoc.TryGetProperty("city", out var c) ? c.GetString() : "";
                    string district = addressLoc.TryGetProperty("district", out var d) ? d.GetString() : "";
                    string county = addressLoc.TryGetProperty("county", out var co) ? co.GetString() : "";
                    string state = addressLoc.TryGetProperty("state", out var s) ? s.GetString() : "";
                    string postalCode = addressLoc.TryGetProperty("postcode", out var p) ? p.GetString() : "";
                    string country = addressLoc.TryGetProperty("country", out var cn) ? cn.GetString() : "";
                    return $"{country},{state}, {county}, {district}, {neighbourhood},{road}, {city},  {postalCode}";
                }
            }
            catch
            {
            }

            return "";
        }

        [HttpPost]
        [Route("api/[Controller]/CheckNameORMobile")]
        [AllowAnonymous]
        public string CheckNameORMobile(androidCheckNameORMobileModelView request)
        {
            string Mobile;
            //// بررسی ورودی ها
            //if (string.IsNullOrEmpty(request.CustomersName))
            //{
            //    response_login =
            //        new androidLoginResponseModelView(ErrorCode.Conflict.GetHashCode(),
            //            "نام یا نام خانوادگی خالی میباشد");
            //    return JsonConvert.SerializeObject(response_login);
            //}
            if (string.IsNullOrEmpty(request.Mobile))
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره مشتری خالی میباشد");

                //response_login =
                //    new androidLoginResponseModelView(ErrorCode.Conflict.GetHashCode(),
                //        "شماره مشتری خالی میباشد");
                //return JsonConvert.SerializeObject(response_login);
            }
            // ----------------------------------
            //CustomersName = request.CustomersName;
            Mobile = request.Mobile;
            SqlParameter[] parameters =
            {
                //new SqlParameter("CustomersName",CustomersName),
                new SqlParameter("Mobile",Mobile),
            };
            DataTable dt = cp.select_row("API_CheckCustomersNameORmobile", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                if (dt.Rows.Count == 1)
                {
                    if (dt.Rows[0][0].ToString() == "CustomersExists")
                    {
                        return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نام کامل مشتری تکراری میباشد");

                        //                response_login =
                        //new androidLoginResponseModelView(ErrorCode.Conflict.GetHashCode(),
                        //    "نام کامل مشتری تکراری میباشد");
                        //                return JsonConvert.SerializeObject(response_login);
                    }
                    else if (dt.Rows[0][0].ToString() == "MobileExists")
                    {
                        return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره مشتری تکراری میباشد");

                        //                response_login =
                        //new androidLoginResponseModelView(ErrorCode.Conflict.GetHashCode(),
                        //    "شماره مشتری تکراری میباشد");
                        //                return JsonConvert.SerializeObject(response_login);
                    }
                    ListResponseAll list = new ListResponseAll();
                    List<crmOnly1StrModel> CheckNameORMobile = new List<crmOnly1StrModel>();

                    //foreach (DataRow row in dt.Rows)
                    //{
                    CheckNameORMobile.Add(new crmOnly1StrModel()
                    {
                        Resualt = "OK",//row[0].ToString(),
                    });
                    //}
                    list = new ListResponseAll()
                    {
                        List = CheckNameORMobile
                    };

                    if (CheckNameORMobile.Count <= 0)
                    {
                        list = new ListResponseAll()
                        {
                            List = new List<crmCityResponseModel>()
                        };
                    }
                    response_City = new crmCityResponseModelViewALL(list);
                    return JsonConvert.SerializeObject(response_City);
                }
                else
                {
                    response_login = new androidLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "خطا در شناسایی نام مشتری یا شماره مشتری");
                }
            }

            return JsonConvert.SerializeObject(response_login);
        }


        [HttpPost]
        [Route("api/[Controller]/Customers")]
        [Authorize]
        public string Customers(androidCustomersRequestModelView request)
        {
            string BirthDate = request.BirthDate;
            string Description = request.Description;
            string CustomersName = request.Name;
            string PostalCode = "";
            string CustomersLastName = request.LastName;
            bool TypeCustomers = request.TypeCustomers;
            int IdCountry = request.IdCountry;
            int IdCity = request.IdCity;
            int IdOstan = request.IdOstan;
            int? ItemAdditonId1 =
                request.ItemAdditonId1 > 0 ? request.ItemAdditonId1 : null;

            int? ItemAdditonId2 =
                request.ItemAdditonId2 > 0 ? request.ItemAdditonId2 : null;


            ItemAdditonId1 = (ItemAdditonId1 == 0) ? null : ItemAdditonId1;
            ItemAdditonId2 = (ItemAdditonId2 == 0) ? null : ItemAdditonId2;
            string mobile = request.mobile;
            string address = request.Address;
            int UserId = HttpContext.Items["UserId"].ToInt();

            if (string.IsNullOrEmpty(address))
            {
                address = "آدرس مشتری موبایل";
            }

            if (mobile.Length == 8 || 
               (mobile.Length == 11 & mobile.StartsWith("09")) ||
               (mobile.Length == 12 & mobile.StartsWith("98")) ||
               (mobile.Length == 13 & mobile.StartsWith("+9")) ||
               (mobile.Length == 11 & !mobile.StartsWith("09")) ||
               (mobile.Length < 11 & mobile.StartsWith("0") & !mobile.StartsWith("09")))
            {

            }
            else
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "فرمت شماره نامعتبر میباشد");
            }
            //            string mobilecheck =
            //            case when len(@mobile) = 8 then @mobile
            //when len(@mobile)= 11 and @mobile LIKE '09%' then @mobile
            //when len(@mobile)= 12 and @mobile LIKE '98%' then '0' + substring(@mobile, 3, 10)
            //when len(@mobile)= 13 and @mobile LIKE '+9%' then '0' + substring(@mobile, 4, 10)
            //when len(@mobile)= 11 and @mobile NOT LIKE '09%' then substring(@mobile,4,8) 
            if (UserId == 0)
            {
                messagebase m = new messagebase()
                {
                    code = ErrorCode.Authentication.GetHashCode(),
                    success = false,
                    message = "عدم احراز هویت"
                };
                return JsonConvert.SerializeObject(m);
            }
            if ( string.IsNullOrEmpty(CustomersName))
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نام مشتری اجباری میباشد");

            }
            if (!TypeCustomers & string.IsNullOrEmpty(CustomersLastName))//حقوقی
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نام خانوادگی مشتری اجباری میباشد");

            }
            if (TypeCustomers)
            { CustomersLastName = ""; }
            if (string.IsNullOrEmpty(mobile))
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "موبایل مشتری اجباری میباشد");
            }
            if (string.IsNullOrEmpty(IdOstan.ToString()) || IdOstan == 0)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "انتخاب استان اجباری میباشد");
            }
            if (string.IsNullOrEmpty(IdCity.ToString()) || IdCity == 0)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "انتخاب شهر اجباری میباشد");

            }

            //if (string.IsNullOrEmpty(Description))
            //{
            //    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "توضیحات اجباری میباشد");
            //}


            SqlParameter[] parameters3 =
                { 
            new SqlParameter("CustomersName", CustomersName ?? (object)DBNull.Value),
                    new SqlParameter("CustomersLastName", CustomersLastName ?? (object)DBNull.Value),
                    new SqlParameter("TypeCustomers", TypeCustomers),
                    new SqlParameter("IdCity", IdCity),
                    new SqlParameter("IdOstan", IdOstan),
                    new SqlParameter("IdCountry", IdCountry),
                    new SqlParameter("ItemAdditonId1", ItemAdditonId1?? (object)DBNull.Value),
                    new SqlParameter("ItemAdditonId2", ItemAdditonId2?? (object)DBNull.Value),
                    new SqlParameter("mobile", mobile ?? (object)DBNull.Value),
                    new SqlParameter("address", address ?? (object)DBNull.Value),
                    new SqlParameter("PostalCode", ""),
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("Description", Description ?? (object)DBNull.Value),
                    new SqlParameter("Birthdate", BirthDate ?? (object)DBNull.Value)
                };
            string id = cp.insert_getid("SPandroid_Createcustomers_insert", parameters3);
            if (cls_public.sqlError.success == false)
            {
                response_Customer = new androidOnly1StrModelResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(),
                    cls_public.sqlError.message);
            }
            else
            {
                if (id != "0" & id != "RepeatCustomersName" & id != "RepeatNumber")
                {
                    crmOnly1StrModel data = new crmOnly1StrModel();
                    data.Resualt = id.ToString();
                    if (id.ToString().Contains("IX"))
                    {
                        if (id.ToString().Contains("IX_PRG_Tell"))
                        {
                            return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره تلفن تکراری می باشد");
                        }
                        else
                            return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), id.ToString());

                    }
                    response_Customer = new androidOnly1StrModelResponseModelView(data);
                }
                else if (id == "RepeatCustomersName")
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "مشتری با عنوان مورد نظر در سیستم وجود دارد");
                }
                else if (id == "RepeatNumber")
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "مشتری با شماره موبایل مورد نظر در سیستم وجود دارد");
                }
                else
                {
                    if (cls_public.sqlError.message == "NotFoundGuid")
                        response_Customer = new androidOnly1StrModelResponseModelView(ErrorCode.BadRequest.GetHashCode(),
                            $"{cls_public.sqlError.message}");
                    else if (cls_public.sqlError.message == "DuplicateGuid")
                        response_Customer = new androidOnly1StrModelResponseModelView(ErrorCode.Duplicate.GetHashCode(),
                            $"{cls_public.sqlError.message}");
                    else
                        response_Customer = new androidOnly1StrModelResponseModelView(ErrorCode.DBO_Error.GetHashCode(),
                            $"{cls_public.sqlError.message}");

                }
            }
            return JsonConvert.SerializeObject(response_Customer);
        }

        [HttpPost]
        [Route("api/[Controller]/call")]
        [Authorize]
        public string call(androidCallRequestModelView request)
        {
            string CustomerFullName = "";
            string CRC_createdate = request.date;
            string CRC_TimeCall = request.time;
            string mobile = "";// request.mobile;
            int State = request.state;
            int Type = request.type;
            int UserId = HttpContext.Items["UserId"].ToInt();
            string Duration = request.duration;

            if (UserId == 0)
            {
                messagebase m = new messagebase()
                {
                    code = ErrorCode.Authentication.GetHashCode(),
                    success = false,
                    message = "عدم احراز هویت"
                };
                return JsonConvert.SerializeObject(m);
            }

            checkmobile = true;
            mobile = NormalizeMobile(request.mobile);
            if (checkmobile)
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("USR_id", UserId),
                };
                //Classes.Library.WriteErrorLog(" mobile CallOnly :" + mobile);

                DataTable dt = cp.select_row("SPandroid_GetAccessUser", parameters);
                SqlParameter[] parameters2 =
                {
                    new SqlParameter("Mobile", mobile),
                };
                DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByMobile", parameters2);
                if (dt_InfoCustomer.Rows.Count > 0)
                {
                   // Classes.Library.WriteErrorLog(" CustomersName CallOnly :" + dt_InfoCustomer.Rows[0]["fullname"].ToString());
                    CustomerFullName = dt_InfoCustomer.Rows[0]["fullname"].ToString();
                }
                //else
                //    Classes.Library.WriteErrorLog(" Customer mobile (" + mobile +") Notfound");

                SqlParameter[] parameters3 =
                    {
                    new SqlParameter("CRC_createdate", CRC_createdate ?? (object)DBNull.Value),
                    new SqlParameter("CRC_TimeCall", CRC_TimeCall ?? (object)DBNull.Value),
                    new SqlParameter("mobile", mobile ?? (object)DBNull.Value),
                    new SqlParameter("State", State),
                    new SqlParameter("Type", Type),
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("Duration", Duration ?? (object)DBNull.Value),
                };
                //Classes.Library.WriteErrorLog("Call Only : " + CRC_createdate.ToString() + " ** " +
                //            CRC_TimeCall.ToString() + " ** " +
                //            mobile.ToString() + " ** " //+
                            //State.ToString() + " ** " +
                            //Type.ToString() + " ** " +
                            //UserId.ToString() + " ** " +
                            //Duration.ToString() + " ** "
                            //);
                string id = cp.insert_getid("SPandroid_CreateCall_insert", parameters3);
                if (cls_public.sqlError.success == false)
                {
                    response_call = new androidCallResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(),
                        cls_public.sqlError.message);
                }
                else
                {
                   // Classes.Library.WriteErrorLog("Call Only Succeed: " + CRC_createdate.ToString() + " ** " +
                            //CRC_TimeCall.ToString() + " ** " +
                            //mobile.ToString() + " ** " 
                            //);
                    if (id != "0" && dt.Rows.Count >= 1)
                    {
                        androidCallResponseModel data = new androidCallResponseModel();
                        data.id = id.ToInt();
                        data.CustomersName = CustomerFullName;

                        data.access = new androidCallResponseModel.data()
                        {
                            SoundRecord = dt.Rows[0]["USR_SoundRecord"].ToBoolean(),
                            GpsLocation = dt.Rows[0]["USR_GpsLocation"].ToBoolean(),
                        };
                        response_call = new androidCallResponseModelView(data);
                    }
                    else
                    {
                        if (cls_public.sqlError.message == "NotFoundGuid")
                            response_call = new androidCallResponseModelView(ErrorCode.BadRequest.GetHashCode(),
                                $"{cls_public.sqlError.message}");
                        else if (cls_public.sqlError.message == "DuplicateGuid")
                            response_call = new androidCallResponseModelView(ErrorCode.Duplicate.GetHashCode(),
                                $"{cls_public.sqlError.message}");
                        else
                            response_call = new androidCallResponseModelView(ErrorCode.DBO_Error.GetHashCode(),
                                $"{cls_public.sqlError.message}");

                    }
                }
                return JsonConvert.SerializeObject(response_call);
            }
            else
            {
                messagebase mes = new messagebase();
                mes.success = false;
                mes.code = ErrorCode.Conflict.GetHashCode();
                mes.message = "شماره مشتری نامعتبر میباشد";
                return JsonConvert.SerializeObject(mes);
            }

        }
        bool checkmobile = true;

        [HttpPost]
        [Route("api/[Controller]/call/list")]
        [Authorize]
        public string callList([FromBody] List<androidCallRequestAllModelView> requests)
        {
            List<androidCallResponseAllModel> responses = new();

            int UserId = HttpContext.Items["UserId"].ToInt();

            if (UserId == 0)
            {
                return JsonConvert.SerializeObject(new messagebase()
                {
                    code = ErrorCode.Authentication.GetHashCode(),
                    success = false,
                    message = "عدم احراز هویت"
                });
            }

            // دسترسی‌های کاربر (یک بار)
            SqlParameter[] parameters =
            {
        new SqlParameter("USR_id", UserId),
    };
            //Classes.Library.WriteErrorLog(" SPandroid_GetAccessUser :" );

            DataTable dtAccess = cp.select_row("SPandroid_GetAccessUser", parameters);
            var customerNameCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var request in requests)
            {
                try
                {
                    string CustomerFullName = "";
                    checkmobile = true;
                    //Classes.Library.WriteErrorLog("NormalizeMobile mobile: "+ request.mobile);
                    string mobile = NormalizeMobile(request.mobile);
                    // 
                    if (checkmobile)
                    {
                        if (!customerNameCache.TryGetValue(mobile, out CustomerFullName))
                        {
                            SqlParameter[] parameters2 =
                            {
                              new SqlParameter("Mobile", mobile),
                            };

                            DataTable dt_InfoCustomer =
                                cp.select_row("API_GetCustomerInfoByMobile", parameters2);

                            if (dt_InfoCustomer.Rows.Count > 0)
                            {
                                CustomerFullName = dt_InfoCustomer.Rows[0]["fullname"].ToString();
                            }
                            else
                            {
                                CustomerFullName = "";
                            }
                            customerNameCache[mobile] = CustomerFullName;
                        }

                        //if (!string.IsNullOrEmpty(CustomerFullName))
                        //    Classes.Library.WriteErrorLog(" CustomersName List Mobile :" + CustomerFullName);
                        //else
                        //    Classes.Library.WriteErrorLog(" Customers List Mobile (" + mobile + ") Notfound");
                        
                        //Classes.Library.WriteErrorLog("Before Insert Call List Mobile: " + CustomerFullName);
                        //Classes.Library.WriteErrorLog("request.state: " + request.state);
                        //Classes.Library.WriteErrorLog("request.type: " + request.type);
                        //Classes.Library.WriteErrorLog("user_id: " + UserId);

                        SqlParameter[] parameters3 =
                        {
                            new SqlParameter("CRC_createdate", request.date ?? (object)DBNull.Value),
                            new SqlParameter("CRC_TimeCall", request.time ?? (object)DBNull.Value),
                            new SqlParameter("mobile", mobile ?? (object)DBNull.Value),
                            new SqlParameter("State", request.state),
                            new SqlParameter("Type", request.type),
                            new SqlParameter("user_id", UserId),
                            new SqlParameter("Duration", request.duration ?? (object)DBNull.Value),
                        };
                        
                        string id = cp.insert_getid("SPandroid_CreateCall_insert", parameters3);

                        if (cls_public.sqlError.success == false)
                        {
                            response_call_all = new androidCallResponseAllModelView(
                                    ErrorCode.DBO_Unavailable.GetHashCode(),
                                    cls_public.sqlError.message

                            );
                        }
                        else if (id != "0" && dtAccess.Rows.Count >= 1)
                        {
                            androidCallResponseAllModel data = new androidCallResponseAllModel()
                            {
                                id = id.ToInt(),
                                LocalId = request.id,
                                CustomersName = CustomerFullName,
                            };

                            responses.Add(data);
    //                        Classes.Library.WriteErrorLog("Insert Call List Succeed: " + request.date.ToString() + " ** " +
    //request.time.ToString() + " ** " +
    //mobile.ToString() + " ** "
                                //);
                            //response_call_all = new androidCallResponseAllModelView(data);
                        }
                        else
                        {
    //                        Classes.Library.WriteErrorLog("Insert Call List Error: " + request.date.ToString() + " ** " +
    //request.time.ToString() + " ** " +
    //mobile.ToString() + " ** "
    //                            );
                            response_call_all = new androidCallResponseAllModelView(
                                    ErrorCode.DBO_Error.GetHashCode(),
                                    cls_public.sqlError.message
                            );
                        }
                    }
                    else
                    {
                        //Classes.Library.WriteErrorLog(" False :" + mobile);
                        response_call_all =
                            new androidCallResponseAllModelView(
                                ErrorCode.BadRequest.GetHashCode(),
                                "شماره موبایل اعتبار ندارد"

                        );
                    }
                }
                catch (Exception ex)
                {
                    //Classes.Library.WriteErrorLog(" Insert Call List Error :" + ex.Message);

                    response_call_all =
                        new androidCallResponseAllModelView(
                            ErrorCode.DBO_Error.GetHashCode(),
                            ex.Message

                    );
                }
            }
            //Classes.Library.WriteErrorLog(" responses.Count :" + responses.Count.ToString());
            if (responses.Count > 0)
                response_call_all = new androidCallResponseAllModelView(responses);

            return JsonConvert.SerializeObject(response_call_all);
        }

        [HttpPost]
        [Route("api/[Controller]/call/update/list")]
        [Authorize]
        public string callUpdateList( [FromBody] List<androidCallRequestAllModelView.androidUpdateCallRequestModelView> requests)
        {
            List<messagebase> responses = new();
            if (requests == null || requests.Count == 0)
            {
                return JsonConvert.SerializeObject(new messagebase()
                {
                    success = false,
                    code = ErrorCode.BadRequest.GetHashCode(),
                    message = "لیست درخواست خالی است"
                });
            }
           // Classes.Library.WriteErrorLog(" Update Call List :");

            foreach (var request in requests)
            {
                messagebase mes = new();

                try
                {
                    if (request.call_id <= 0)
                    {
                        mes.success = false;
                        mes.code = ErrorCode.BadRequest.GetHashCode();
                        mes.message = "call_id نامعتبر است";
                        responses.Add(mes);
                        continue;
                    }
                    //Classes.Library.WriteErrorLog("1:"+ request.location);
                    string LocationAddress = "";
                   // Classes.Library.WriteErrorLog(" Update Call List :"); 
                    if (!string.IsNullOrEmpty(request.location))
                    {
                        //Classes.Library.WriteErrorLog("2");
                        try
                        {
                            var http = new HttpClient();
                            var strloc = request.location.Split(',');
                            string lat = strloc[0];
                            string lon = strloc[1];
                            

                            http.DefaultRequestHeaders.UserAgent
                                .ParseAdd("SigmaLocation/1.0 (SigmaGroup1403@Gmail.com)");

                            string url =
                                $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}";

                            var response = http.GetStringAsync(url).Result;
                            using var doc = JsonDocument.Parse(response);

                            if (doc.RootElement.TryGetProperty("address", out var addressLoc))
                            {
                                string road = addressLoc.TryGetProperty("road", out var r) ? r.GetString() : "";
                                string neighbourhood = addressLoc.TryGetProperty("neighbourhood", out var n) ? n.GetString() : "";
                                string city = addressLoc.TryGetProperty("city", out var c) ? c.GetString() : "";
                                string district = addressLoc.TryGetProperty("district", out var d) ? d.GetString() : "";
                                string county = addressLoc.TryGetProperty("county", out var co) ? co.GetString() : "";
                                string state = addressLoc.TryGetProperty("state", out var s) ? s.GetString() : "";
                                string PostalCode = addressLoc.TryGetProperty("postcode", out var p) ? p.GetString() : "";
                                string country = addressLoc.TryGetProperty("country", out var cn) ? cn.GetString() : "";

                                //از ادرس مشتری ترکیب رشته مرتب و خوانا
                                LocationAddress = $"{country},{state}, {county}, {district}, {neighbourhood},{road}, {city},  {PostalCode}";
                            }

                        }
                        catch { }
                    }
                    //                    SqlParameter[] parameters =
                    //{
                    //                        new SqlParameter("CRC_id", request.call_id),
                    //                        new SqlParameter("cammands", request.commands.TryString() ?? (object)DBNull.Value),
                    //                        new SqlParameter("alarm_date", DBNull.Value),
                    //                        new SqlParameter("alarm_time", request.alarm_time ?? (object)DBNull.Value),
                    //                        new SqlParameter("alarm_desc", request.alarm_desc.TryString() ?? (object)DBNull.Value),
                    //                        new SqlParameter("location", request.location.TryString() ?? (object)DBNull.Value),
                    //                        new SqlParameter("LocationAddress", LocationAddress.TryString() ?? (object)DBNull.Value)
                    //                    };
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("CRC_id", request.call_id),
                        new SqlParameter("cammands", request.commands.TryString() ?? (object)DBNull.Value),
                        new SqlParameter("alarm_date", DBNull.Value),
                        new SqlParameter("alarm_time", DBNull.Value),
                        new SqlParameter("alarm_desc",DBNull.Value),
                        new SqlParameter("location",  request.location.TryString() ?? (object)DBNull.Value),
                        new SqlParameter("LocationAddress", LocationAddress.TryString() ?? (object)DBNull.Value)
                    };
                    // Classes.Library.WriteErrorLog("8:" + request.alarm_date.ToString());
                    //Classes.Library.WriteErrorLog("81:" + request.alarm_time.ToString());
                    //Classes.Library.WriteErrorLog("82:" + request.call_id.ToString());
                    //Classes.Library.WriteErrorLog(" Update Call List :" + request.alarm_date.ToString() + " ** " +
                    //     request.alarm_time.ToString() + " ** " +
                    //     request.call_id.ToString() + " ** "
                    //     );
                    if (cp.update("SPandroid_CreateCall_update", parameters))
                    {
                       // Classes.Library.WriteErrorLog("10:");
                       // Classes.Library.WriteErrorLog(" Update Call List Succeed:" + request.call_id.ToString());
                        mes.success = true;
                        mes.code = 201;
                        mes.message = "ویرایش انجام شد";
                        responses.Add(mes);
                    }
                    else
                    {
                        //Classes.Library.WriteErrorLog("11:");
                       // Classes.Library.WriteErrorLog(" call_id Not Found in Update:" + request.call_id.ToString()) ;
                        mes.success = true;
                        mes.code = 201;
                        mes.message = "تماس یافت نشد";
                        responses.Add(mes); 
                        //mes.success = false;
                        //mes.code = ErrorCode.DBO_Unavailable.GetHashCode();
                        //mes.message = cls_public.sqlError.message;
                        //responses.Add(mes);
                    }
                }
                catch (Exception ex)
                {
                   // Classes.Library.WriteErrorLog(" Update Call List Error:" + ex.Message);

                    mes.success = false;
                    mes.code = 400;
                    mes.message = ex.Message;
                    responses.Add(mes);
                }
            }

            return JsonConvert.SerializeObject(responses[0]);
        }


        [HttpPost]
        [Route("api/[Controller]/call/UpdateReferralsummary/list/{call_id}/{ListItem}")]
        [Authorize]
        public string callUpdateReferralsummary(int call_id, string ListItem)
        {
            List<messagebase> responses = new();
            messagebase mes = new();

            try
            {
                if (call_id <= 0)
                {
                    mes.success = false;
                    mes.code = ErrorCode.BadRequest.GetHashCode();
                    mes.message = "call_id نامعتبر است";
                    responses.Add(mes);
                    //continue;
                }
                else
                {
                    DataTable DtValidFiledatabaseSummary = new DataTable();
                    DtValidFiledatabaseSummary.Columns.Add("CallSummary", typeof(int));
                    DtValidFiledatabaseSummary.Columns.Add("lkp", typeof(int));
                    string[] str = ListItem.Split(',');
                    foreach (var request in str)
                    {
                        DtValidFiledatabaseSummary.Rows.Add(DBNull.Value, request);
                    }
                    SqlParameter[] parameters =
                    {
                    new SqlParameter("Call", call_id),
                    new SqlParameter("edit", false),
                }; 
                   
                        ;
                    if (cp.updateDatatable("SPInPRG_CallSummary", parameters, "@TableCallSummary", DtValidFiledatabaseSummary))
                    {
                        //Classes.Library.WriteErrorLog(" Update Referralsummary List Succeed:" + call_id.ToString());
                        mes.success = true;
                        mes.code = 201;
                        mes.message = "";
                        responses.Add(mes);
                        return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "");
                    }
                    else
                    {
                        //Classes.Library.WriteErrorLog("11:");
                        //Classes.Library.WriteErrorLog(" call_id Not Found in Referralsummary:" + call_id.ToString());
                        mes.success = false;
                        mes.code = 202;
                        mes.message = "تماس یافت نشد";
                        responses.Add(mes);
                        //mes.success = false;
                        //mes.code = ErrorCode.DBO_Unavailable.GetHashCode();
                        //mes.message = cls_public.sqlError.message;
                        //responses.Add(mes);
                    }
                }
            }
            catch (Exception ex)
            {
                //Classes.Library.WriteErrorLog(" Referralsummary Call List Error:" + ex.Message);

                mes.success = false;
                mes.code = 400;
                mes.message = ex.Message;
                responses.Add(mes);
            }

            return JsonConvert.SerializeObject(responses[0]);
        }

        [HttpPost]
        [Route("api/[Controller]/call/UpdateReasonsfornotbuying/list/{call_id}/{ListItem}")]
        [Authorize]
        public string callUpdateReasonsfornotbuying(int call_id, string ListItem)
        {
            List<messagebase> responses = new();
            messagebase mes = new();

            try
            {
                if (call_id <= 0)
                {
                    mes.success = false;
                    mes.code = ErrorCode.BadRequest.GetHashCode();
                    mes.message = "call_id نامعتبر است";
                    responses.Add(mes);
                    //continue;
                }
                else
                {
                    DataTable DtValidFiledatabaseReasonsfornotbuying = new DataTable();
                    DtValidFiledatabaseReasonsfornotbuying.Columns.Add("CallSummary", typeof(int));
                    DtValidFiledatabaseReasonsfornotbuying.Columns.Add("lkp", typeof(int));
                    string[] str = ListItem.Split(',');
                    foreach (var request in str)
                    {
                        DtValidFiledatabaseReasonsfornotbuying.Rows.Add(DBNull.Value, request);
                    }
                    SqlParameter[] parameters =
                    {
                    new SqlParameter("Call", call_id),
                    new SqlParameter("edit", false),
                };

                    if (cp.updateDatatable("SPInPRG_CustomersNeedCall", parameters, "@TableCustomersNeed", DtValidFiledatabaseReasonsfornotbuying))
                    {
                       // Classes.Library.WriteErrorLog(" Update databaseReasonsfornotbuying List Succeed:" + call_id.ToString());
                        mes.success = true;
                        mes.code = 201;
                        mes.message = "";
                        responses.Add(mes);
                        return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "");
                    }
                    else
                    {
                       // Classes.Library.WriteErrorLog(" call_id Not Found in databaseReasonsfornotbuying:" + call_id.ToString());
                        mes.success = true;
                        mes.code = 202;
                        mes.message = "تماس یافت نشد";
                        responses.Add(mes);

                    }
                }
            }
            catch (Exception ex)
            {
               // Classes.Library.WriteErrorLog(" databaseReasonsfornotbuying Call List Error:" + ex.Message);

                mes.success = false;
                mes.code = 400;
                mes.message = ex.Message;
                responses.Add(mes);
            }
            return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "اطلاعات ارسال شده ناقص میباشد");

            return JsonConvert.SerializeObject(responses[0]);
        }

        [HttpPost]
        [Route("api/[Controller]/call/Batch")]
        [Authorize]
        public string call([FromBody] List<androidCallBatchRequestModelView> request)
        {
            bool check = true;
            bool error = false;
            string CustomerFullName = "";
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "عدم احراز هویت");
            }
            if (request == null || !request.Any())
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "اطلاعات ارسال شده ناقص میباشد");
            }

            var locationAddresses = new Dictionary<string, string>();
            var customerNameCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in request)
            {
                if (item.Description != null && !string.IsNullOrEmpty(item.Description.location))
                    locationAddresses[item.guid] = ResolveBatchLocationAddress(item.Description.location);
            }

            SqlTransaction transaction = cp.Transaction("CallBatch");
            List<androidCallBatchResponseModel> callBatch = new List<androidCallBatchResponseModel>();
            foreach (androidCallBatchRequestModelView item in request)
            {
                try
                {

                    if (string.IsNullOrEmpty(item.guid))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.mobile))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.date))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.time))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.duration))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.state.ToString()))
                    {
                        check = false;
                        break;
                    }
                    if (string.IsNullOrEmpty(item.type.ToString()))
                    {
                        check = false;
                        break;
                    }
                    checkmobile = true;

                    string mobile = NormalizeMobile(item.mobile);
                    if (checkmobile)
                    {
                        if (!customerNameCache.TryGetValue(item.mobile, out CustomerFullName))
                        {
                            SqlParameter[] parameters2 =
                            {
                                new SqlParameter("Mobile", item.mobile),
                            };
                            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByMobile", parameters2, transaction);
                            CustomerFullName = dt_InfoCustomer.Rows.Count > 0
                                ? dt_InfoCustomer.Rows[0]["fullname"].ToString()
                                : "";
                            customerNameCache[item.mobile] = CustomerFullName;
                        }

                        SqlParameter[] parameters3 =
                        {
                    new SqlParameter("CRC_createdate", item.date ?? (object)DBNull.Value),
                    new SqlParameter("CRC_TimeCall", item.time ?? (object)DBNull.Value),
                    new SqlParameter("mobile", item.mobile ?? (object)DBNull.Value),
                    new SqlParameter("State", item.state),
                    new SqlParameter("Type", item.type),
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("Duration", item.duration ?? (object)DBNull.Value),
                };
                        string id = cp.insert_getid("SPandroid_CreateCall_insert", parameters3, transaction);
                        if (id != "0")
                        {


                            if (item.Description == null)
                            {
                                callBatch.Add(new androidCallBatchResponseModel()
                                {
                                    status = true,
                                    guid = item.guid,
                                    CustomersName = CustomerFullName,
                                    id = id.ToInt()
                                });
                            }
                            else
                            {
                                string LocationAddress = "";
                                if (item.Description != null && !string.IsNullOrEmpty(item.Description.location))
                                {
                                    locationAddresses.TryGetValue(item.guid, out LocationAddress);
                                    LocationAddress = LocationAddress ?? "";
                                }
                                SqlParameter[] parameters =
                                {
                            new SqlParameter("CRC_id", id),
                            new SqlParameter("cammands", item.Description.commands ?? (object)DBNull.Value),
                            new SqlParameter("alarm_date", item.Description.alarm_date ?? (object)DBNull.Value),
                            new SqlParameter("alarm_time", item.Description.alarm_time ?? (object)DBNull.Value),
                            new SqlParameter("alarm_desc", item.Description.alarm_desc ?? (object)DBNull.Value),
                            new SqlParameter("location", item.Description.location ?? (object)DBNull.Value),
                            new SqlParameter("LocationAddress",LocationAddress.TryString() ?? (object)DBNull.Value)

                        };

                                if (cp.update("SPandroid_CreateCall_update", parameters, transaction))
                                {
                                    callBatch.Add(new androidCallBatchResponseModel()
                                    {
                                        status = true,
                                        guid = item.guid,
                                        CustomersName = CustomerFullName,
                                        id = null,
                                    });
                                }
                                else
                                {
                                    callBatch.Add(new androidCallBatchResponseModel()
                                    {
                                        status = true,
                                        guid = item.guid,
                                        CustomersName = CustomerFullName,
                                        id = id.ToInt()
                                    });
                                }
                            }
                        }
                        else
                        {
                            callBatch.Add(new androidCallBatchResponseModel()
                            {
                                status = false,
                                guid = item.guid,
                                CustomersName = CustomerFullName,
                                id = null
                            });
                        }
                    }
                    else
                    {
                        messagebase mes = new messagebase();
                        mes.success = false;
                        mes.code = ErrorCode.Conflict.GetHashCode();
                        mes.message = "شماره مشتری نامعتبر میباشد";
                        return JsonConvert.SerializeObject(mes);
                    }
                }
                catch (Exception e)
                {
                    check = false;
                    error = true;
                    break;
                }
            }
            if (cls_public.sqlError.success == false)
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "پایگاه داده در دسترس نیست");
            if (check)
            {
                transaction.Commit();
                transaction.Dispose();
                response_callBatch = new androidCallBatchResponseModelView(callBatch);
                return JsonConvert.SerializeObject(response_callBatch);
            }
            else
            {
                transaction.Rollback();
                transaction.Dispose();
                if (error)
                    if (cls_public.sqlError.message != null)
                        return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "اطلاعات ارسال شده ناقص میباشد");
                    else
                        return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "عملیات با موفقیت انجام نشد");
                else
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "اطلاعات ارسال شده ناقص میباشد");
            }
        }

        [HttpPost]
        [Route("api/[Controller]/call/{call_id?}")]
        [Authorize]
        public string call_update(androidCallRequestModelView.androidUpdateCallRequestModelView request)
        {
            messagebase mes = new messagebase();

            if (!Request.RouteValues.TryGetValue("call_id", out var callIdObj) || callIdObj == null)
            {
                mes.success = false;
                mes.code = ErrorCode.BadRequest.GetHashCode();
                mes.message = "not found call_id";
                return JsonConvert.SerializeObject(mes);
            }

            int call_id = Request.RouteValues["call_id"].ToInt();
            //string call_id = Request.RouteValues["call_id"].ToString();
            string cammands = request.commands;
            string alarm_date = request.alarm_date;
            string alarm_time = request.alarm_time;
            string alarm_desc = request.alarm_desc;
            string location = request.location;
            string LocationAddress = "";
            string[] strloc;
            if (!string.IsNullOrEmpty(location))
            {
                try
                {
                    var http = new HttpClient();
                    strloc = location.Split(',');
                    string lat = strloc[0].ToString();// 31.8768633,54.3588895;
                    string lon = strloc[1].ToString();//54.3588895;

                    http.DefaultRequestHeaders.UserAgent.ParseAdd("SigmaLocation/1.0 (SigmaGroup1403@Gmail.com)");
                    string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={lat}&lon={lon}";

                    var response = http.GetStringAsync(url).GetAwaiter().GetResult();
                    using var doc = JsonDocument.Parse(response);
                    var root = doc.RootElement;
                    if (root.TryGetProperty("address", out var addressLoc))
                    {
                        string road = addressLoc.TryGetProperty("road", out var r) ? r.GetString() : "";
                        string neighbourhood = addressLoc.TryGetProperty("neighbourhood", out var n) ? n.GetString() : "";
                        string city = addressLoc.TryGetProperty("city", out var c) ? c.GetString() : "";
                        string district = addressLoc.TryGetProperty("district", out var d) ? d.GetString() : "";
                        string county = addressLoc.TryGetProperty("county", out var co) ? co.GetString() : "";
                        string state = addressLoc.TryGetProperty("state", out var s) ? s.GetString() : "";
                        string PostalCode = addressLoc.TryGetProperty("postcode", out var p) ? p.GetString() : "";
                        string country = addressLoc.TryGetProperty("country", out var cn) ? cn.GetString() : "";

                        //از ادرس مشتری ترکیب رشته مرتب و خوانا
                        LocationAddress = $"{country},{state}, {county}, {district}, {neighbourhood},{road}, {city},  {PostalCode}";
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Error:");
                    //Console.WriteLine(ex.Message);
                }
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("CRC_id",call_id),
                new SqlParameter("cammands",cammands.TryString() ?? (object)DBNull.Value ),
                new SqlParameter("alarm_date",alarm_date ?? (object)DBNull.Value),
                new SqlParameter("alarm_time",alarm_time ?? (object)DBNull.Value),
                new SqlParameter("alarm_desc",alarm_desc.TryString() ?? (object)DBNull.Value),
                new SqlParameter("location",location.TryString() ?? (object)DBNull.Value),
                new SqlParameter("LocationAddress",LocationAddress.TryString() ?? (object)DBNull.Value)
            };

            if (cp.update("SPandroid_CreateCall_update", parameters))
            {
                mes.success = true;
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    mes.success = cls_public.sqlError.success;
                    mes.code = ErrorCode.DBO_Unavailable.GetHashCode();
                    mes.message = cls_public.sqlError.message;
                }
                else
                {
                    mes.success = false;
                    mes.code = ErrorCode.Conflict.GetHashCode();
                    mes.message = "ویرایش اطلاعات با مشکل مواجه شد";
                }

            }

            return JsonConvert.SerializeObject(mes);
        }


        [HttpPost]
        [Route("api/[Controller]/call/{call_id}/location")]
        [Authorize]
        public string call_update_location(androidCallRequestModelView.androidUpdateCallLocationRequestModelView request)
        {
            messagebase mes = new messagebase();

            int call_id = Request.RouteValues["call_id"].ToInt();
            //string call_id = Request.RouteValues["call_id"].ToString();
            string location = request.location;
            SqlParameter[] parameters =
            {
                new SqlParameter("CRC_id",call_id),
                new SqlParameter("location",location.TryString() ?? (object)DBNull.Value)
            };
            if (cp.update("SPandroid_LocationCall_update", parameters))
            {
                mes.success = true;
                mes.code = ErrorCode.Created.GetHashCode();
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    mes.success = cls_public.sqlError.success;
                    mes.code = ErrorCode.DBO_Unavailable.GetHashCode();
                    mes.message = cls_public.sqlError.message;
                }
                else
                {
                    mes.success = false;
                    mes.code = ErrorCode.Conflict.GetHashCode();
                    mes.message = "ثبت موقعیت مکانی با مشکل مواجه شد";
                }
            }

            return JsonConvert.SerializeObject(mes);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Citys")]
        public string CityList([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_city = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCityResponseModel> citys = new List<crmCityResponseModel>();

            string baseQuery = $"SELECT LocationId,Parent,dbo.ArabicPersion(Title) Title FROM {clsProperties.GetPropertise("DataMali").ToString()}.GNR.Location WHERE Type = 3";
            dt_city = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_city.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_city.Rows)
                {
                    citys.Add(new crmCityResponseModel()
                    {
                        Id = row["LocationId"].ToInt(),
                        Title = row["Title"].ToString(),
                        Parent = row["Parent"].ToInt()
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Ostan")]
        public string OstanList([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_Ostan = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCityResponseModel> citys = new List<crmCityResponseModel>();

            string baseQuery = $"SELECT LocationId,Parent,dbo.ArabicPersion(Title) Title FROM {clsProperties.GetPropertise("DataMali").ToString()}.GNR.Location WHERE Type = 2";
            dt_Ostan = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Ostan.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Ostan.Rows)
                {
                    citys.Add(new crmCityResponseModel()
                    {
                        Id = row["LocationId"].ToInt(),
                        Title = row["Title"].ToString(),
                        Parent = row["Parent"].ToInt()
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Country")]
        public string CountryList([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_Country = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCityResponseModel> citys = new List<crmCityResponseModel>();

            string baseQuery = $"SELECT LocationId,Parent,dbo.ArabicPersion(Title) Title FROM {clsProperties.GetPropertise("DataMali").ToString()}.GNR.Location WHERE Type = 1";
            dt_Country = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Country.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Country.Rows)
                {
                    citys.Add(new crmCityResponseModel()
                    {
                        Id = row["LocationId"].ToInt(),
                        Title = row["Title"].ToString(),
                        Parent = row["Parent"].ToInt()
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/CustomersList")]
        public string CustomersList([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_Customers = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCustomersResponseModel> citys = new List<crmCustomersResponseModel>();
            string DataMali = clsProperties.GetPropertise("DataMali").ToString();
            string baseQuery = @"select cst_id,dlsep.Code,isnull( cst_fname,'')+' '+ ISNULL( cst_laname ,'')+' ('+loccity.Title+')' CustomersName from PRG_Customers 
  join " + DataMali + @".gnr.Location loccity on loccity.LocationId=cst_city 
            JOIN [dbo].[PRG_Tell] ON tel_cst_id=cst_id and tel_type=1 
            LEFT JOIN " + DataMali + ".acc.DL dlsep on dlsep.DLId = csT_dl";
            dt_Customers = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Customers.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Customers.Rows)
                {
                    citys.Add(new crmCustomersResponseModel()
                    {
                        ID = row["cst_id"].ToInt(),
                        Code = row["Code"].ToString(),
                        Title = row["CustomersName"].ToString()
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCustomersResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [HttpPost]
        [Route("api/[Controller]/SetNotificationCall")]
        [Authorize]
        public string SetNotificationCall(androidAddNofiticationRequestModelView request)
        {
            messagebase mes = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "عدم احراز هویت");
            }
            //if (!Request.RouteValues.TryGetValue("call_id", out var callIdObj) || callIdObj == null)
            //{
            //    mes.success = false;
            //    mes.code = ErrorCode.BadRequest.GetHashCode();
            //    mes.message = "not found call_id";
            //    return JsonConvert.SerializeObject(mes);
            //}




            //int call_id = Request.RouteValues["call_id"].ToInt();
            //string call_id = Request.RouteValues["call_id"].ToString();
            int CSTID = request.CSTID;
            string alarm_date = request.alarm_date;
            string alarm_time = request.alarm_time;
            string alarm_desc = request.alarm_desc;
            SqlParameter[] parameters =
            {
                new SqlParameter("Usrid",UserId),
                new SqlParameter("CStid",CSTID),
                new SqlParameter("alarm_date",alarm_date ?? (object)DBNull.Value),
                new SqlParameter("alarm_time",alarm_time ?? (object)DBNull.Value),
                new SqlParameter("alarm_desc",alarm_desc.TryString() ?? (object)DBNull.Value),
            };

            if (cp.update("SPandroid_CreateNotifications", parameters))
            {
                mes.success = true;
            }
            else
            {
                if (cls_public.sqlError.success == false)
                {
                    mes.success = cls_public.sqlError.success;
                    mes.code = ErrorCode.DBO_Unavailable.GetHashCode();
                    mes.message = cls_public.sqlError.message;
                }
                else
                {
                    mes.success = false;
                    mes.code = ErrorCode.Conflict.GetHashCode();
                    mes.message = "ثبت نوتیفیکیشن با مشکل مواجه شد";
                }

            }

            return JsonConvert.SerializeObject(mes);
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Referralsummary")]
        public string Referralsummary([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_Referralsummary = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmReferralsummaryORcrmReferralsummaryResponseModel> citys = new List<crmReferralsummaryORcrmReferralsummaryResponseModel>();

            string baseQuery = $"SELECT LKP_id,cast(0 as bit) id, LKP_name FROM PRG_Loockup WHERE LKP_enownerName ='CallSummary' and LKP_state=1 order by LKP_id";
            dt_Referralsummary = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Referralsummary.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Referralsummary.Rows)
                {
                    citys.Add(new crmReferralsummaryORcrmReferralsummaryResponseModel()
                    {
                        Id = row["LKP_id"].ToInt(),
                        Check = row["id"].ToBoolean(),
                        Title = row["LKP_name"].ToString(),
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }


        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Reasonsfornotbuying")]
        public string Reasonsfornotbuying([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_Reasonsfornotbuying = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmReferralsummaryORcrmReferralsummaryResponseModel> citys = new List<crmReferralsummaryORcrmReferralsummaryResponseModel>();

            string baseQuery = $"select LKP_id,cast(0 as bit) id, LKP_name from dbo.PRG_Loockup where LKP_state=1 AND LKP_enownerName='NotBuyCustomer' and LKP_state=1 order by LKP_id";
            dt_Reasonsfornotbuying = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Reasonsfornotbuying.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Reasonsfornotbuying.Rows)
                {
                    citys.Add(new crmReferralsummaryORcrmReferralsummaryResponseModel()
                    {
                        Id = row["LKP_id"].ToInt(),
                        Check = row["id"].ToBoolean(),
                        Title = row["LKP_name"].ToString(),
                    });
                }
                list = new ListResponseAll()
                {
                    List = citys
                };
            }
            if (citys.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/AdditionItem1")]
        public string AdditionItem1([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_AdditionItem1 = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCityResponseModel> AdditionItem1 = new List<crmCityResponseModel>();

            string baseQuery = $"select LKP_id,dbo.ArabicPersion(LKP_name) LKP_name from PRG_Loockup where LKP_enownerName='CustomerGroup'";
            dt_AdditionItem1 = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            //Classes.Library.WriteErrorLog(" AdditionItem1 " + dt_AdditionItem1.Rows.Count.ToString());

            if (dt_AdditionItem1.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_AdditionItem1.Rows)
                {
                    AdditionItem1.Add(new crmCityResponseModel()
                    {
                        Id = row["LKP_id"].ToInt(),
                        Title = row["LKP_name"].ToString()
                    });
                }
                list = new ListResponseAll()
                {
                    List = AdditionItem1
                };
            }
            if (AdditionItem1.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/ListAlarm/{AlarmDateStr}")]
        public string ListAlarm([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_ListAlarm = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmListAlarmResponseModel> Alarmlists = new List<crmListAlarmResponseModel>();
            int UserId = HttpContext.Items["UserId"].ToInt();
            string AlarmDateStr = Request.RouteValues["AlarmDateStr"].ToString(); 
            if (UserId == 0)
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "عدم احراز هویت");
            }


            string baseQuery = @"select SCC_id,SCC_date AlarmDate ,OPC_desc Customers,OPC_cst Customersid, SCC_desc descriptions from PRG_SchedureCustomers 
join PRG_OpportunityCustomers on OPC_id=SCC_opc_id where SCC_opc_id is not null 
and SCC_Showdate is null and SCC_CRC_id is null and SCC_typeme is null  and convert(date,SCC_date) = convert(date,'" + AlarmDateStr + "') AND SCC_usr_id=" + UserId.ToString();
            dt_ListAlarm = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_ListAlarm == null)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else if (dt_ListAlarm.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_ListAlarm.Rows)
                {
                    Alarmlists.Add(new crmListAlarmResponseModel()
                    {
                        SCC_id = row["SCC_id"].ToInt(),
                        AlarmDate = Convert.ToDateTime( row["AlarmDate"].ToString()),
                        Customers = row["Customers"].ToString(),
                        Customersid = row["Customersid"].ToInt(),
                        descriptions = row["descriptions"].ToString(),
                    });
                }
                list = new ListResponseAll()
                {
                    List = Alarmlists
                };
            }
            if (Alarmlists.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/ListBirthDate")]
        public string ListBirthDate([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_BirthDate = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmListBirthDateResponseModel> BirthDate = new List<crmListBirthDateResponseModel>();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "عدم احراز هویت");
            }
            bool customeruser = false;
            SqlParameter[] parameters =
            {
                new SqlParameter("id",UserId)
            };
            DataTable dtaccess = cp.select_row("SpSelectAccessBirthDate", parameters);

            foreach (DataRow dr in dtaccess.Rows)
            {
                if (dr["acc_name"].ToString() == "customeruser")
                {
                    if (Convert.ToBoolean(dr["acc_Type"].ToString()))
                    {
                        customeruser = true;
                    }
                    else
                    {
                        customeruser = false;
                    }
                }
            }
            
//            string matn = "";
//            string baseQuery = @"SELECT(SELECT   top(1)  tel_tell
//                            FROM          PRG_Tell
//                            WHERE(tel_type = 1) AND(tel_cst_id = cst.cst_id)) AS Phone,dbo.mtsh(cst.cst_birthdate) birthdateShamsi, cst.cst_birthdate birthdate, cst_id Customersid, cst.cst_fname +' ' + cst.cst_laname AS CustomersName
//FROM PRG_Customers cst
//WHERE DATEDIFF(DAY,
//               CAST(GETDATE() AS DATE),
//               CASE
//                   WHEN DATEADD(YEAR, YEAR(GETDATE()) - YEAR(cst.cst_birthdate), cst.cst_birthdate) >= CAST(GETDATE() AS DATE)
//                   THEN DATEADD(YEAR, YEAR(GETDATE()) -YEAR(cst.cst_birthdate), cst.cst_birthdate)
//                   ELSE DATEADD(YEAR, YEAR(GETDATE()) -YEAR(cst.cst_birthdate) + 1, cst.cst_birthdate)
//               END)
//BETWEEN 1 AND 7 "; 
//            if (customeruser)
//            {
//                matn = baseQuery + @" and  (cst.cst_id IN
//                          (SELECT     opc.OPC_cst
//                            FROM          PRG_UserAssignCustomers AS us INNER JOIN
//                                                   PRG_OpportunityCustomers AS opc ON us.UAC_opc_id = opc.OPC_id
//                            WHERE (us.UAC_usr_id = " + UserId.ToString() + ") AND(us.UAC_type = 1) AND (us.UAC_datekhatemeh IS NULL)))";
//            }
//            else
//            {
//                matn = baseQuery + @"and ((cst.cst_id NOT IN
//                          (SELECT     opc.OPC_cst
//                            FROM          PRG_UserAssignCustomers AS us INNER JOIN
//                                                   PRG_OpportunityCustomers AS opc ON us.UAC_opc_id = opc.OPC_id
//                            WHERE (us.UAC_usr_id != " + UserId.ToString() + @") AND us.UAC_exclusive = 1 AND us.UAC_type = 1 AND us.UAC_datekhatemeh IS NULL))
//							OR 
//							(cst.cst_id IN
//                          (SELECT     opc.OPC_cst
//                            FROM          PRG_UserAssignCustomers AS us INNER JOIN
//                                                   PRG_OpportunityCustomers AS opc ON us.UAC_opc_id = opc.OPC_id
//                            WHERE (us.UAC_usr_id = " + UserId.ToString() + @") AND us.UAC_exclusive = 1 AND us.UAC_type = 1 AND us.UAC_datekhatemeh IS NULL))
//							)";
//            }
            SqlParameter[] parametersBirth =
            {
                new SqlParameter("customeruser",customeruser),
                new SqlParameter("id",UserId)
            };
            dt_BirthDate = cp.select_row("SpSelectBirthDateCustomers", parametersBirth);

            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_BirthDate == null)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else if (dt_BirthDate.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_BirthDate.Rows)
                {
                    BirthDate.Add(new crmListBirthDateResponseModel()
                    {
                        Customersid = row["Customersid"].ToInt(),
                        birthdate = Convert.ToDateTime(row["birthdate"].ToString()),
                        birthdateShamsi = row["birthdateShamsi"].ToString(),
                        CustomersName = row["CustomersName"].ToString(),
                        Phone = row["Phone"].ToString(),
                    });
                }
                list = new ListResponseAll()
                {
                    List = BirthDate
                };
            }
            if (BirthDate.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/ListBirthDatePersonel")]
        public string ListBirthDatePersonel([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_BirthDate = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmListBirthDatePersonelResponseModel> BirthDate = new List<crmListBirthDatePersonelResponseModel>();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "عدم احراز هویت");
            }

            dt_BirthDate = cp.select("SpSelectBirthDatePersonel");

            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_BirthDate == null)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else if (dt_BirthDate.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_BirthDate.Rows)
                {
                    BirthDate.Add(new crmListBirthDatePersonelResponseModel()
                    {
                        PersonelId = row["PersonelID"].ToInt(),
                        birthdate = Convert.ToDateTime(row["birthdate"].ToString()),
                        birthdateShamsi = row["birthdateShamsi"].ToString(),
                        PersonelIdName = row["PersonelName"].ToString(),
                    });
                }
                list = new ListResponseAll()
                {
                    List = BirthDate
                };
            }
            if (BirthDate.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/AdditionItem2")]
        public string AdditionItem2([FromQuery] ListResponseAll request)
        {
            messagebase m = new messagebase();
            DataTable dt_AdditionItem2 = new DataTable();
            int Pages = 1;
            ListResponseAll list = new ListResponseAll();
            List<crmCityResponseModel> AdditionItem2 = new List<crmCityResponseModel>();

            string baseQuery = $"select LKP_id,dbo.ArabicPersion(LKP_name) LKP_name from PRG_Loockup where LKP_enownerName='CustomerGroup2'";
            dt_AdditionItem2 = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
           // Classes.Library.WriteErrorLog(" AdditionItem2" + dt_AdditionItem2.Rows.Count.ToString());

            if (dt_AdditionItem2.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_AdditionItem2.Rows)
                {
                    AdditionItem2.Add(new crmCityResponseModel()
                    {
                        Id = row["LKP_id"].ToInt(),
                        Title = row["LKP_name"].ToString()
                    });
                }
                list = new ListResponseAll()
                {
                    List = AdditionItem2
                };
            }
            if (AdditionItem2.Count <= 0)
            {
                list = new ListResponseAll()
                {
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelViewALL(list);
            return JsonConvert.SerializeObject(response_City);
        }
        private string NormalizeMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return mobile;
            else if (mobile.StartsWith("98") || mobile.StartsWith("+98") || mobile.StartsWith("09") || mobile.StartsWith("9"))
            {
                if (mobile.StartsWith("98"))
                {
                    mobile = "+" + mobile;
                    mobile = mobile.Replace("+98", "0");
                }

                else if (mobile.StartsWith("+98"))
                {
                    mobile = mobile.Replace("+98", "0");
                }
                else if (mobile.StartsWith("9"))
                {
                    mobile = "0" + mobile;
                }
                if (mobile.Length < 11)
                    checkmobile = false;
            }
            else
            {
                foreach (string code in new clsProperties().provincePhoneCodes)
                {
                    if (mobile.StartsWith(code))
                    {
                        if (mobile.Length > 3)
                            mobile = mobile.Substring(3, mobile.Length - 3);
                        break;
                    }
                }
            }


            //if (mobile.StartsWith("98"))
            //    mobile = "0" + mobile.Substring(2);

            //else if (mobile.StartsWith("+98"))
            //    mobile = "0" + mobile.Substring(3);

            //else if (!mobile.StartsWith("09"))
            //{
            //    foreach (string code in new clsProperties().provincePhoneCodes)
            //    {
            //        if (mobile.StartsWith(code))
            //        {
            //            mobile = mobile.Replace(code, "");
            //            break;
            //        }
            //    }
            //}

            return mobile;
        }
    }
}
