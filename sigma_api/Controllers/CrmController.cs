using Azure;
using FastReport;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Drives.Item.Items.Item.GetActivitiesByIntervalWithStartDateTimeWithEndDateTimeWithInterval;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Models;
using sigma_api.ModelView;
using sigma_api.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
namespace sigma_api.Controllers
{
    public class CrmController : Controller
    {
        cls_public cp = new cls_public();
        ShamsiDate ShD = new ShamsiDate();
        crmLoginResponseModelView response_login;
        crmRegistryCodeResponseModelView response_RegistryCode;
        crmItemListResponseModelView response_items;
        crmQuotationsResponseModelView response_quotations;
        generalInfoCustomerResponseModelView response_InfoCustomer;
        crmBillResponseModelView response_bill;
        crmCycleRefferingResponseModelView response_Cycle;
        crmDashboardResponseModelView response_dashboard;
        crmCityResponseModelView response_City;

        private readonly IWebHostEnvironment _env;
        public CrmController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public string ImageConverter(byte[] file, string filename)
        {
            try
            {

                string folderPath = Path.Combine(_env.WebRootPath == null ? _env.ContentRootPath : _env.WebRootPath, "images-item");

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, filename);
                string imageUrl = null;
                if (file != null)
                    if (file.Length > 0)
                    {

                        if (!System.IO.File.Exists(filePath))
                        {
                            System.IO.File.WriteAllBytes(filePath, file);
                        }

                        imageUrl = $"{Request.Scheme}://{Request.Host}/images-item/{filename}";
                    }

                return imageUrl;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        // Optimized version that takes pre-validated folder path to avoid repeated directory checks
        public string OptimizedImageConverter(byte[] file, string filename, string folderPath, string baseImageUrl)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                string filePath = Path.Combine(folderPath, filename);

                // Only write file if it doesn't exist
                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllBytes(filePath, file);
                }

                return baseImageUrl + filename;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("api/[controller]/login")]
        [AllowAnonymous]
        public string login([FromBody] crmLoginRequestModelView request)
        {
            string username = "";
            string password;
            if (!string.IsNullOrEmpty(request.username))
                username = request.username = PersianNumbersToEnglish(request.username);
            password = coding.SigmaEncryptString(/*coding.SafeFarsiStr(*/PersianNumbersToEnglish(request.password)/*)*/);
            if (cp.select_command($"SELECT * FROM dbo.PRG_Tell WHERE tel_tell = '{username}' AND tel_type = 1").Rows.Count <= 0)
            {
                return messagebase.messages(false, ErrorCode.InvalidUser.GetHashCode(), "حساب کاربری یافت نشد");
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("username",username),
                new SqlParameter("password",password),
                new SqlParameter("Type",1),
            };
            DataTable dt = cp.select_row("API_CheckUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                response_login = new crmLoginResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {
                if (dt.Rows.Count == 1)
                {
                    int ID = dt.Rows[0]["ID"].ToInt();
                    string FirstName = dt.Rows[0]["FirstName"].ToString();
                    string LastName = dt.Rows[0]["LastName"].ToString();
                    string Birthdate = dt.Rows[0]["Birthdate"].ToString();
                    string MeliCode = dt.Rows[0]["MeliCode"].ToString();
                    string Email = dt.Rows[0]["Email"].ToString();
                    string Type = dt.Rows[0]["Type"].ToString();
                    bool Status = dt.Rows[0]["Status"].ToBoolean();
                    if (Status == false)
                    {
                        response_login = new crmLoginResponseModelView(ErrorCode.NoAccess.GetHashCode(), "حساب کاربری غیرفعال میباشد");
                    }
                    else
                    {
                        string token = TokenManagement.WriteToken(ID, TypeToken.customer.GetHashCode());
                        if (TokenManagement.IsCreated)
                        {
                            crmLoginResponseModel data = new crmLoginResponseModel();
                            data.id = token;
                            data.Type = Type;
                            data.FirstName = FirstName;
                            data.LastName = LastName;
                            data.MeliCode = MeliCode;
                            data.Email = Email;
                            data.Birthdate = Birthdate;
                            response_login = new crmLoginResponseModelView(data);
                            HttpContext.Response.Headers.Add("Token", token);
                        }
                        else
                        {
                            response_login =
                                new crmLoginResponseModelView(ErrorCode.Authentication.GetHashCode(),
                                    "احراز هویت با مشکل مواجه شد");
                        }

                    }
                }
                else
                {
                    response_login = new crmLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }
            return JsonConvert.SerializeObject(response_login);
        }

        [HttpPost("api/[controller]/create-account")]
        [AllowAnonymous]
        public string signup()
        {
            return "";
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/dashboard")]
        public string GetStatusPortal()
        {
            messagebase mes = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();

            SqlParameter[] parameters =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_sales = cp.select_row("API_Sales6Month", parameters);
            List<crmDashboardResponseModel.clsSales> Sales = new List<crmDashboardResponseModel.clsSales>();
            if (dt_sales.Rows.Count > 0)
            {
                foreach (DataRow row in dt_sales.Rows)
                {
                    Sales.Add(new crmDashboardResponseModel.clsSales()
                    {
                        date = row["PersianMonth"].ToString(),
                        value = row["TotalSales"].ToDecimal()
                    });
                }
            }

            SqlParameter[] parameters1 =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_ComparisonSales = cp.select_row("API_ComparisonSales6Month", parameters1);
            List<crmDashboardResponseModel.clsSales> ComparisonSales = new List<crmDashboardResponseModel.clsSales>();
            if (dt_ComparisonSales.Rows.Count > 0)
            {
                foreach (DataRow row in dt_ComparisonSales.Rows)
                {
                    ComparisonSales.Add(new crmDashboardResponseModel.clsSales()
                    {
                        date = row["PersianMonth"].ToString(),
                        value = row["TotalSales"].ToDecimal()
                    });
                }
            }
            SqlParameter[] parameters2 =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_stats = cp.select_row("API_GetStatusPortal", parameters2);


            crmDashboardResponseModel.clsStatsValues invoice = new crmDashboardResponseModel.clsStatsValues();
            crmDashboardResponseModel.clsStatsValues quotation = new crmDashboardResponseModel.clsStatsValues();
            crmDashboardResponseModel.clsStats Stats = new crmDashboardResponseModel.clsStats();

            if (dt_stats.Rows.Count > 0)
            {
                invoice = new crmDashboardResponseModel.clsStatsValues()
                {
                    total = dt_stats.Rows[0]["INV_all"].ToInt(),
                    approved = dt_stats.Rows[0]["INV_confrim"].ToInt(),
                    cancelled = dt_stats.Rows[0]["INV_ebtal"].ToInt(),
                    pending = dt_stats.Rows[0]["INV_inProgress"].ToInt()
                };
                quotation = new crmDashboardResponseModel.clsStatsValues()
                {
                    total = dt_stats.Rows[0]["QU_all"].ToInt(),
                    approved = dt_stats.Rows[0]["QU_confrim"].ToInt(),
                    cancelled = dt_stats.Rows[0]["QU_ebtal"].ToInt(),
                    pending = dt_stats.Rows[0]["QU_inProgress"].ToInt()
                };
                Stats = new crmDashboardResponseModel.clsStats()
                {
                    invoice = invoice,
                    quotation = quotation
                };
            }



            crmDashboardResponseModel res = new crmDashboardResponseModel()
            {
                ComparisonSalesChart = ComparisonSales,
                SalesChart = Sales,
                Stats = Stats,
            };
            response_dashboard = new crmDashboardResponseModelView(res);
            return JsonConvert.SerializeObject(response_dashboard);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/get-info-customer")]
        public string getInfoCustomer()
        {
            messagebase mes = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters2 =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByIdCrm", parameters2);
            if (dt_InfoCustomer.Rows.Count > 0)
            {
                generalInfoCustomerResponseModel data = new generalInfoCustomerResponseModel();
                data.Id = dt_InfoCustomer.Rows[0]["ID"].ToInt();
                data.FirstName = dt_InfoCustomer.Rows[0]["FirstName"].ToString();
                data.LastName = dt_InfoCustomer.Rows[0]["LastName"].ToString();
                data.FullName = dt_InfoCustomer.Rows[0]["fullname"].ToString(); ;
                data.Birthdate = dt_InfoCustomer.Rows[0]["Birthdate"].ToString();
                data.MeliCode = dt_InfoCustomer.Rows[0]["MeliCode"].ToString();
                data.Email = dt_InfoCustomer.Rows[0]["Email"].ToString();
                data.Type = dt_InfoCustomer.Rows[0]["Type"].ToString();
                data.TypeId = dt_InfoCustomer.Rows[0]["TypeId"].ToInt();
                data.Mobile = dt_InfoCustomer.Rows[0]["Mobile"].ToString();
                data.Address = dt_InfoCustomer.Rows[0]["Address"].ToString();
                data.Status = dt_InfoCustomer.Rows[0]["Status"].ToInt();
                data.SalesExpert = dt_InfoCustomer.Rows[0]["SalesExpert"].ToInt();
                data.SalesExpert_FullName = dt_InfoCustomer.Rows[0]["SalesExpert_name"].ToString();
                data.city = dt_InfoCustomer.Rows[0]["cst_city"].ToInt();
                data.cityName = dt_InfoCustomer.Rows[0]["CityName"].ToString();
                data.CustomersCode = dt_InfoCustomer.Rows[0]["CustomersCode"].ToString();

                response_InfoCustomer = new generalInfoCustomerResponseModelView(data);
            }
            else
            {
                response_InfoCustomer = new generalInfoCustomerResponseModelView(null);
            }
            return JsonConvert.SerializeObject(response_InfoCustomer);
        }

        [HttpPost]
        [Route("api/[controller]/forgot-password")]
        [AllowAnonymous]
        public string RegistryCode([FromBody] crmRegistryCodeRequestModelView request)
        {
            string mobile = "";
            int? code = null;
            mobile = request.mobile;
            try
            {
                code = request.code <= 0 || request.code is null ? null : request.code;
            }
            catch
            {
                code = null;
            }

            if (mobile.StartsWith("98"))
            {
                mobile = "+" + mobile;
                mobile = mobile.Replace("+98", "0");
            }

            if (mobile.StartsWith("+98"))
            {
                mobile = mobile.Replace("+98", "0");
            }
            if (cp.select_command($"SELECT * FROM dbo.PRG_Tell WHERE tel_tell = '{mobile}' AND tel_type = 1").Rows.Count <= 0)
            {
                return messagebase.messages(false, ErrorCode.InvalidUser.GetHashCode(), "حساب کاربری یافت نشد");
            }
            SqlParameter[] parameters2 =
            {
                new SqlParameter("Mobile", mobile),
                new SqlParameter("RegistryCode", string.IsNullOrEmpty(code.ToString())?DBNull.Value:code),
            };
            string res_code = cp.insert_getid("API_RegistryCodeUsers", parameters2).ToString();
            if (res_code.StartsWith("1-"))
            {
                string Company = clsProperties.GetPropertise("txtnameforoshgah").ToString();

                int RandCode = res_code.Split('-')[1].ToInt();
                clsSms.send(mobile, $"کد تایید فراموشی رمز عبور پورتال مشتریان : {RandCode}" +"\n\r"+ Company);
                response_RegistryCode = new crmRegistryCodeResponseModelView(true, ErrorCode.Send.GetHashCode(), "ارسال کد تایید");
            }
            else
            {
                if (res_code.ToInt() == 0)
                {
                    response_RegistryCode = new crmRegistryCodeResponseModelView(false, ErrorCode.Conflict.GetHashCode(),
                        "تایید حساب کاربری با مشکل مواجه شده است");
                }
                else if (res_code.ToInt() > 1)
                {

                    string token = TokenManagement.WriteToken(res_code.ToInt(), TypeToken.customer.GetHashCode());
                    if (TokenManagement.IsCreated)
                    {
                        crmLoginResponseModel data = new crmLoginResponseModel();
                        data.id = token;
                        response_login = new crmLoginResponseModelView(data);
                        response_RegistryCode = new crmRegistryCodeResponseModelView(true, ErrorCode.Created.GetHashCode(), "registered", data);
                    }
                    else
                    {
                        response_RegistryCode = new crmRegistryCodeResponseModelView(false, ErrorCode.Conflict.GetHashCode(),
                            "تایید حساب کاربری با مشکل مواجه شده است");
                    }
                }
            }

            return JsonConvert.SerializeObject(response_RegistryCode);
        }

        [Authorize]
        [HttpPost]
        [Route("api/[controller]/reset-password")]
        public string CheangPassword([FromBody] crmResetPasswordRequestModelView request)
        {
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            try
            {
                if (string.IsNullOrWhiteSpace(request.new_password))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "new_password ارائه نشده است.");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("UserId", UserId),
                new SqlParameter("Password", coding.SigmaEncryptString(request.new_password)),
            };
            if (cp.update("API_CheangPasswordUser", parameters))
            {
                m.code = ErrorCode.Created.GetHashCode();
                m.success = true;
                m.message = "کلمه عبور با موفقیت تغییر کرد";
            }
            else
            {
                m.code = ErrorCode.Conflict.GetHashCode();
                m.success = false;
                m.message = "تغییر کلمه عبور با مشکل مواجه شد";
            }
            return JsonConvert.SerializeObject(m);
        }

        //[Authorize]
        //[HttpPut]
        //[Route("api/[controller]/reset-password")]
        //public string CheangPassword([FromBody] crmResetPasswordRequestModelView request)
        //{
        //    messagebase m = new messagebase();
        //    int UserId = HttpContext.Items["UserId"].ToInt();
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(request.new_password))
        //        {
        //            return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "new_password ارائه نشده است.");
        //        }
        //    }
        //    catch
        //    {
        //        return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
        //    }
        //    SqlParameter[] parameters =
        //    {
        //        new SqlParameter("UserId", UserId),
        //        new SqlParameter("Password", coding.SigmaEncryptString(request.new_password)),
        //    };
        //    if (cp.update("API_CheangPasswordUser", parameters))
        //    {
        //        m.code = ErrorCode.Created.GetHashCode();
        //        m.success = true;
        //        m.message = "کلمه عبور با موفقیت تغییر کرد";
        //    }
        //    else
        //    {
        //        m.code = ErrorCode.Conflict.GetHashCode();
        //        m.success = false;
        //        m.message = "تغییر کلمه عبور با مشکل مواجه شد";
        //    }
        //    return JsonConvert.SerializeObject(m);
        //}

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Citys")]
        public string CityList([FromQuery] ListRequest request)
        {
            try
            {
                if (request.page_number < 1)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
            }
            messagebase m = new messagebase();
            DataTable dt_city = new DataTable();
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmCityResponseModel> citys = new List<crmCityResponseModel>();

            // Build the SQL query with proper parameter handling
            string baseQuery = $"SELECT LocationId,dbo.ArabicPersion(Title) Title FROM {clsProperties.GetPropertise("DataMali").ToString()}.GNR.Location WHERE Type = 3";

            if (!string.IsNullOrEmpty(request.Search) && request.Search.Trim().Length > 0)
            {
                // URL decode the search parameter to handle Persian characters properly
                string decodedSearch = System.Web.HttpUtility.UrlDecode(request.Search);

                // Use parameterized query to prevent SQL injection
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@SearchTerm", $"%{decodedSearch.Trim()}%")
                };

                baseQuery += " AND dbo.ArabicPersion(Title) LIKE @SearchTerm";
                dt_city = cp.select_command(baseQuery, parameters);
            }
            else
            {
                dt_city = cp.select_command(baseQuery);
            }
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
                Pages = dt_city.Rows.Count / 100;
                if (Pages < 1)
                    Pages = 1;
                if (dt_city.Rows.Count > 100)
                {
                    dt_city = dt_city.AsEnumerable().Skip((request.page_number - 1) * 100).CopyToDataTable();
                    dt_city = dt_city.AsEnumerable().Take(100).CopyToDataTable();
                }
                foreach (DataRow row in dt_city.Rows)
                {
                    citys.Add(new crmCityResponseModel()
                    {
                        Id = row["LocationId"].ToInt(),
                        Title = row["Title"].ToString()
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = request.page_number,
                    List = citys
                };

            }
            if (citys.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = request.page_number,
                    List = new List<crmCityResponseModel>()
                };
            }
            response_City = new crmCityResponseModelView(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GroupItemList")]
        public string GroupItemList()
        {
            messagebase m = new messagebase();
            DataTable dt_GroupItem = new DataTable();
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmGroupItemResponseModel> GroupItem = new List<crmGroupItemResponseModel>();

            string baseQuery = $"SELECT LKP_id Id,dbo.ArabicPersion(LKP_name)  Title FROM [dbo].[PRG_Loockup] WHERE LKP_enownerName = 'GroupItem'";

            dt_GroupItem = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_GroupItem.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                Pages = 1;
                foreach (DataRow row in dt_GroupItem.Rows)
                {
                    GroupItem.Add(new crmGroupItemResponseModel()
                    {
                        Id = row["Id"].ToInt(),
                        Title = row["Title"].ToString()
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row =1,
                    List = GroupItem
                };

            }
            if (GroupItem.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,// request.page_number,
                    List = new List<crmGroupItemResponseModel>()
                };
            }
            response_City = new crmCityResponseModelView(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/ItemAdition1List")]
        public string ItemAdition1List()
        {
            messagebase m = new messagebase();
            DataTable dt_GroupItem = new DataTable();
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmGroupItemResponseModel> GroupItem = new List<crmGroupItemResponseModel>();

            string baseQuery = $"SELECT LKP_id Id,dbo.ArabicPersion(LKP_name)  Title FROM [dbo].[PRG_Loockup] WHERE LKP_enownerName = 'ItemAdition1'";

            dt_GroupItem = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_GroupItem.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                Pages = 1;
                foreach (DataRow row in dt_GroupItem.Rows)
                {
                    GroupItem.Add(new crmGroupItemResponseModel()
                    {
                        Id = row["Id"].ToInt(),
                        Title = row["Title"].ToString()
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = 1,
                    List = GroupItem
                };

            }
            if (GroupItem.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,// request.page_number,
                    List = new List<crmGroupItemResponseModel>()
                };
            }
            response_City = new crmCityResponseModelView(list);
            return JsonConvert.SerializeObject(response_City);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/ItemAdition2List")]
        public string ItemAdition2List()
        {
            messagebase m = new messagebase();
            DataTable dt_GroupItem = new DataTable();
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmGroupItemResponseModel> GroupItem = new List<crmGroupItemResponseModel>();

            string baseQuery = $"SELECT LKP_id Id,dbo.ArabicPersion(LKP_name)  Title FROM [dbo].[PRG_Loockup] WHERE LKP_enownerName = 'ItemAdition2'";

            dt_GroupItem = cp.select_command(baseQuery);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_GroupItem.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                Pages = 1;
                foreach (DataRow row in dt_GroupItem.Rows)
                {
                    GroupItem.Add(new crmGroupItemResponseModel()
                    {
                        Id = row["Id"].ToInt(),
                        Title = row["Title"].ToString()
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = 1,
                    List = GroupItem
                };

            }
            if (GroupItem.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,// request.page_number,
                    List = new List<crmGroupItemResponseModel>()
                };
            }
            response_City = new crmCityResponseModelView(list);
            return JsonConvert.SerializeObject(response_City);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Items")]
        public string ItemList([FromQuery] ListItemRequest request)
        {
            
            try
            {
                if (request.NextPage < 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Method not allowed");
            }
            int NumberOfRow = 200;
            //int Pages = 1;
            //Pages = request.page_number;
            int IS_id = 0;
            ListResponse list = new ListResponse();
            List<crmItemListResponseModel> ItemList = new List<crmItemListResponseModel>(NumberOfRow);
            messagebase m = new messagebase();
            int UserId = 0;
            if (HttpContext.Items["UserId"] != null)
            {
                UserId = HttpContext.Items["UserId"].ToInt();
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "User not authenticated");
            }
            DataTable dt_items = new DataTable();
            bool Checkinventory = clsProperties.GetPropertise("NotShowInventoryPortal").ToBoolean();

            string where = "";

            //if (request.SearchType == "0")
            //{
            //    if (!string.IsNullOrEmpty(request.SearchName))
            //        where = " AND " + classes.clsGlobal.StringSplitOptionsRemoveEmptyEntries(request.SearchName);
            //}

            //if (request.SearchType == "1")
            //{
            //    where = $" AND ( dbo.ArabicPersion(itm.ITM_fullname) LIKE dbo.ArabicPersion(N'{request.SearchName}%') " +
            //        $" OR itm.ITM_Code LIKE N'{request.SearchName}%' )"; //classes.clsGlobal.StringSplitOptionsRemoveEmptyEntries(request.SearchName);
            //}
            if (!string.IsNullOrEmpty(request.SearchName))
                request.SearchName = PersianNumbersToEnglish(request.SearchName);
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("IS_id",IS_id),
                new SqlParameter("CustomerId",UserId),
                new SqlParameter("searchType",request.SearchType),
                new SqlParameter("search",string.IsNullOrEmpty(request.SearchName) ? DBNull.Value : HttpUtility.UrlDecode(request.SearchName)),
                new SqlParameter("SearchGroupItem", request.SearchGroupItem == 0
                               ? (object)DBNull.Value
                               : (object)request.SearchGroupItem),
                new SqlParameter("SearchItemAddition1", request.SearchItemAddition1 == 0
                               ? (object)DBNull.Value
                               : (object)request.SearchItemAddition1),
                new SqlParameter("SearchItemAddition2", request.SearchItemAddition2 == 0
                               ? (object)DBNull.Value
                               : (object)request.SearchItemAddition2),
                new SqlParameter("NextPage",request.NextPage == 0
                               ? (object)DBNull.Value
                               : (object)request.NextPage ),
                new SqlParameter("PreviousPage",request.PreviousPage == 0
                               ? (object)DBNull.Value
                               : (object)request.PreviousPage ),
                new SqlParameter("NumberOfRow",NumberOfRow),
                new SqlParameter("Checkinventory",Checkinventory),
            };
          //  DataTable  dtms = cp.select_command("select 40000: "+ where);

            dt_items = cp.select_row("API_ListItems", parameters);
           // dtms = cp.select_command("select 50000");
            //return dt_items.Rows.Count.ToString();
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_items == null || dt_items.Rows.Count <= 0)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "کالای مورد نظر یافت نشد");

                //if (cls_public.sqlError.message != null)
                //    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {
                // Setup image directory once outside the loop for better performance
                string imageFolderPath = Path.Combine(_env.WebRootPath == null ? _env.ContentRootPath : _env.WebRootPath, "images-item");
                if (!Directory.Exists(imageFolderPath))
                    Directory.CreateDirectory(imageFolderPath);

                // Cache column existence checks to avoid repeated lookups
                bool hasBrand = dt_items.Columns.Contains("Brand");
                bool hasDistinctCustomersBrand = dt_items.Columns.Contains("DistinctCustomersBrand");
                bool hasUnitId2 = dt_items.Columns.Contains("UnitId2");
                bool hasUnit2 = dt_items.Columns.Contains("Unit2");
                bool hasDuty = dt_items.Columns.Contains("duty");
                bool hasTax = dt_items.Columns.Contains("tax");
                bool hasSupplierId = dt_items.Columns.Contains("SupplierId");
                bool hasSupplier = dt_items.Columns.Contains("Supplier");
                bool hasIS_GroupStock = dt_items.Columns.Contains("IS_GroupStock");
                bool hasGRS_GroupName = dt_items.Columns.Contains("GRS_GroupName");
                bool hasImage = dt_items.Columns.Contains("Image"); 
                // Pre-build base URL to avoid string concatenation in loop
                string baseImageUrl = $"{Request.Scheme}://{Request.Host}/images-item/";

                foreach (DataRow row in dt_items.Rows)
                {
                    ItemList.Add(new crmItemListResponseModel()
                    {
                        IS_id = row["IS_id"].ToInt(),
                        StockID = row["StockID"].ToInt(),
                        StockTitle = row["StockTitle"].ToString(),
                        ITM_id = row["ITM_id"].ToInt(),
                        ITM_Code = row["ITM_Code"].ToString(),
                        Sepidar_ItemId = row["Sepidar_ItemId"].ToInt(),
                        FullName = row["FullName"].ToString(),
                        Brand = hasBrand ? row["Brand"].ToString() : "",
                        DistinctCustomersBrand = hasDistinctCustomersBrand ? row["DistinctCustomersBrand"].ToDouble() : 0,
                        UnitId = row["UnitId"].ToInt(),
                        Unit = row["Unit"].ToString(),
                        UnitId2 = hasUnitId2 ? row["UnitId2"].ToInt() : row["UnitId"].ToInt(),
                        Unit2 = hasUnit2 ? row["Unit2"].ToString() : "",
                        PricingQ = row["PricingQ"].ToDecimal(),
                        ITM_nesabt = row["ITM_nesabt"].ToBoolean(),
                        ITM_nesbatValue = row["ITM_nesbatValue"].ToDouble(),
                        duty = hasDuty ? row["duty"].ToInt() : 0,
                        tax = hasTax ? row["tax"].ToInt() : 0,
                        SupplierId = hasSupplierId ? row["SupplierId"].ToInt() : 0,
                        Supplier = hasSupplier ? row["Supplier"].ToString() : "",
                        IS_GroupStock = hasIS_GroupStock ? row["IS_GroupStock"].ToInt() : 0,
                        GRS_GroupName = hasGRS_GroupName ? row["GRS_GroupName"].ToString() : "",
                        Image = hasImage ? OptimizedImageConverter(string.IsNullOrEmpty(row["Image"].ToString()) ? null : (byte[])row["Image"], row["IS_id"].ToInt() + ".jpg", imageFolderPath, baseImageUrl) : null,
                        InventoryStock = row["InventoryStock"].ToInt(),
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,//Pages,
                    Page_row = 0,//.page_number,
                    List = ItemList
                };
            }
            if (ItemList.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 0,//request.page_number,
                    List = new List<crmItemListResponseModel>()
                };
            }
            response_items = new crmItemListResponseModelView(list);
            return JsonConvert.SerializeObject(response_items);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Item/{IS_id}")]
        public string GetItem(int? IS_id)
        {
            if (IS_id == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ItemId ارائه نشده است.");
            }
            bool Checkinventory = clsProperties.GetPropertise("NotShowInventoryPortal").ToBoolean();

            crmItemListResponseModel ItemInfo = null;
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("IS_id",IS_id),
                    new SqlParameter("CustomerId",UserId),
                    new SqlParameter("Checkinventory",Checkinventory),
            };
            DataTable dt_items = new DataTable();
            dt_items = cp.select_row("API_ListItem", parameters);

            bool hasBrand = dt_items.Columns.Contains("Brand");
            bool hasDistinctCustomersBrand = dt_items.Columns.Contains("DistinctCustomersBrand");
            bool hasUnitId2 = dt_items.Columns.Contains("UnitId2");
            bool hasUnit2 = dt_items.Columns.Contains("Unit2");
            bool hasDuty = dt_items.Columns.Contains("duty");
            bool hasTax = dt_items.Columns.Contains("tax");
            bool hasSupplierId = dt_items.Columns.Contains("SupplierId");
            bool hasSupplier = dt_items.Columns.Contains("Supplier");
            bool hasIS_GroupStock = dt_items.Columns.Contains("IS_GroupStock");
            bool hasGRS_GroupName = dt_items.Columns.Contains("GRS_GroupName");
            bool hasImage = dt_items.Columns.Contains("Image");
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }

            if (dt_items.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                ItemInfo = new crmItemListResponseModel()
                {
                    IS_id = dt_items.Rows[0]["IS_id"].ToInt(),
                    StockID = dt_items.Rows[0]["StockID"].ToInt(),
                    StockTitle = dt_items.Rows[0]["StockTitle"].ToString(),
                    ITM_id = dt_items.Rows[0]["ITM_id"].ToInt(),
                    ITM_Code = dt_items.Rows[0]["ITM_Code"].ToString(),
                    Sepidar_ItemId = dt_items.Rows[0]["Sepidar_ItemId"].ToInt(),
                    FullName = dt_items.Rows[0]["FullName"].ToString(),
                    Brand = hasBrand ? dt_items.Rows[0]["Brand"].ToString() : "",
                    DistinctCustomersBrand = hasDistinctCustomersBrand ? dt_items.Rows[0]["DistinctCustomersBrand"].ToDouble() : 0,
                    UnitId = dt_items.Rows[0]["UnitId"].ToInt(),
                    Unit = dt_items.Rows[0]["Unit"].ToString(),
                    UnitId2 = hasUnitId2 ? dt_items.Rows[0]["UnitId2"].ToInt() : dt_items.Rows[0]["UnitId"].ToInt(),
                    Unit2 = hasUnit2 ? dt_items.Rows[0]["Unit2"].ToString() : "",
                    PricingQ = dt_items.Rows[0]["PricingQ"].ToDecimal(),
                    ITM_nesabt = dt_items.Rows[0]["ITM_nesabt"].ToBoolean(),
                    ITM_nesbatValue = dt_items.Rows[0]["ITM_nesbatValue"].ToDouble(),
                    duty = hasDuty ? dt_items.Rows[0]["duty"].ToInt() : 0,
                    tax = hasTax ? dt_items.Rows[0]["tax"].ToInt() : 0,
                    SupplierId = hasSupplierId ? dt_items.Rows[0]["SupplierId"].ToInt() : 0,
                    Supplier = hasSupplier ? dt_items.Rows[0]["Supplier"].ToString() : "",
                    IS_GroupStock = hasIS_GroupStock ? dt_items.Rows[0]["IS_GroupStock"].ToInt() : 0,
                    GRS_GroupName = hasGRS_GroupName ? dt_items.Rows[0]["GRS_GroupName"].ToString() : "",
                    Image = ImageConverter(string.IsNullOrEmpty(dt_items.Rows[0]["Image"].ToString()) ? null : (byte[])dt_items.Rows[0]["Image"], dt_items.Rows[0]["IS_id"].ToInt() + ".jpg"),
                    InventoryStock = dt_items.Rows[0]["InventoryStock"].ToInt(),
                };
            }
            response_items = new crmItemListResponseModelView(ItemInfo);
            return JsonConvert.SerializeObject(response_items);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GetLinkPdf/{FormID}/{FormType}")]
        public string GetLinkPdf(int? FormID, int? FormType)
        {
            if (FormID == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "FormID ارائه نشده است.");
            }
            CrmSendPdfModel ItemInfo = null;
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();

            DataTable dt_Pdf = new DataTable();
            using (SqlCommand _command = new SqlCommand("select DR_script,DR_existence,DR_ReportData from dbo.PRG_DynamicReport where DR_ShowPortal =1 AND DR_existence = " + FormType, cp.connect()))
            {
                _command.CommandType = CommandType.Text;

                //_command.Parameters.AddWithValue("@type", 0);
                //_command.Parameters.AddWithValue("@FormId", Formid);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_Pdf);
                }
            }
            DataTable dtprint = new DataTable();
            if (dt_Pdf.Rows.Count > 0)
            {
                using (SqlCommand _command2 = new SqlCommand(dt_Pdf.Rows[0]["DR_script"].ToString(), cp.connect()))
                {
                    _command2.CommandType = CommandType.Text;

                    _command2.Parameters.AddWithValue("@Id", FormID);
                    //_command.Parameters.AddWithValue("@FormId", Formid);

                    using (SqlDataAdapter adapter1 = new SqlDataAdapter(_command2))
                    {
                        adapter1.Fill(dtprint);
                    }
                }
                foreach (DataRow dr in dtprint.Rows)
                {
                    try
                    {
                        string num2str = num2sr(dr[dr["num2sr"].ToString()].ToString());
                        dr["num2sr"] = num2str;
                        dr.EndEdit();
                        dr.AcceptChanges();
                    }
                    catch { }
                }
                DataTable dtremind = new DataTable();

                DataTable Dtsign = new DataTable();

                Report report = new Report();
                if (FormType == 3)
                {

                    if (clsProperties.GetPropertise("ShowSignsPrintLadingType").ToInt() == 0)
                        Dtsign = GetSignerForm(report, FormType, FormID.ToString());//چاپ امضاها
                    if (clsProperties.GetPropertise("ShowSignsPrintLadingType").ToInt() == 1)
                    {
                        DataTable DtfindId = new DataTable();
                        using (SqlCommand _command2 = new SqlCommand(@"select LD_QU_id from Prg_Lading where LD_id=" + FormID.ToString(), cp.connect()))
                        {
                            _command2.CommandType = CommandType.Text;
                            using (SqlDataAdapter adapter1 = new SqlDataAdapter(_command2))
                            {
                                adapter1.Fill(DtfindId);
                            }
                        }
                        Dtsign = GetSignerForm(report, 2, DtfindId.Rows[0][0].ToString());//پیش فاکتور
                    }
                    if (clsProperties.GetPropertise("ShowSignsPrintLadingType").ToInt() == 2)
                    {
                        DataTable DtfindId = new DataTable();
                        using (SqlCommand _command2 = new SqlCommand(@"select LD_INV_id from Prg_Lading where LD_id=" + FormID.ToString(), cp.connect()))
                        {
                            _command2.CommandType = CommandType.Text;
                            using (SqlDataAdapter adapter1 = new SqlDataAdapter(_command2))
                            {
                                adapter1.Fill(DtfindId);
                            }
                        }
                        Dtsign = GetSignerForm(report, 6, DtfindId.Rows[0][0].ToString());//پیش فاکتور
                    }
                }
                else
                    Dtsign = GetSignerForm(report, FormType, FormID.ToString());//چاپ امضاها
                DataRow dt_Pdfdr = dt_Pdf.Rows[0];
                DaynamicReportDatabase(dt_Pdfdr, dtprint, Dtsign);

                if (!string.IsNullOrEmpty(ErrorStr))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), ErrorStr);
                }
                if (cls_public.sqlError.success == false)
                {
                    return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
                }
                if (sendfile == null)
                {
                    if (cls_public.sqlError.message != null)
                        return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
                }
                else
                {
                    string text = ImageConverter(sendfile, filename + ".pdf");

                    Classes.Library.WriteErrorLog(" Pdf : " + text);

                    ItemInfo = new CrmSendPdfModel()
                    {
                        Pdf = ImageConverter(sendfile, filename + ".pdf")
                    };
                }
                response_items = new crmItemListResponseModelView(ItemInfo);
                return JsonConvert.SerializeObject(response_items);
            }
            else
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "الگویی برای چاپ فرم طراحی نشده است");

            }
        }
        public string filename = "";
        string ErrorStr = "";
        public void DaynamicReportDatabase(DataRow currentRow, DataTable DtprintDesign, DataTable DtDtsign = null)
        {
            try
            {
                byte[] reportBytes;
                reportBytes = (byte[])currentRow["DR_ReportData"];// (byte[])cmd.ExecuteScalar();
                if (reportBytes == null || reportBytes.Length == 0)
                {
                    //MessageBox.Show("فایل گزارش در دیتابیس یافت نشد یا خالی است.");
                    return;
                }

                Report report = new Report();

                // بارگذاری گزارش از حافظه
                using (MemoryStream ms = new MemoryStream(reportBytes))
                {
                    ms.Position = 0;
                    report.Load(ms);
                }

                // ثبت منبع داده
                report.RegisterData(DtprintDesign, "Report");
                report.GetDataSource("Report").Enabled = true;
                if (DtDtsign != null)
                {
                    report.RegisterData(DtDtsign, "Signs");
                    report.GetDataSource("Signs").Enabled = true;
                }
                // تنظیم پارامترها (در صورت نیاز)
                report.SetParameterValue("Filter", "");
                report.Prepare();
                string ExePath = Assembly.GetEntryAssembly()?.Location;
                string directoryPath = AppContext.BaseDirectory;
                if (!Directory.Exists(directoryPath + "PrintPdf"))
                {
                    Directory.CreateDirectory(directoryPath + "PrintPdf");
                }
                Random rand = new Random();
                string crand = rand.Next(1111, 9999).ToString();
                string downloadsPath = directoryPath + "PrintPdf\\" + "PrintPdf" + "_" + crand+".pdf";
                filename = "PrintPdf" + "_" + crand;
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    // ایجاد اکسپورت PDF
                //    // توجه: اگر کلاس PdfExport را نمی‌شناسد، نام آن را در IntelliSense چک کنید
                //    // ممکن است در نسخه‌های خاص PdfSimpleExport باشد
                //    var PDFSimpleExport = new FastReport.Export.PdfSimple.PDFSimpleExport();

                //    try
                //    {
                //        report.Export(pdfExport, ms);

                //        // تبدیل MemoryStream به آرایه بایت
                //        sendfile = ms.ToArray();
                //    }
                //    catch (Exception ex)
                //    {
                //        // مدیریت خطای احتمالی در حین اکسپورت
                //      //  Console.WriteLine($"خطا در خروجی PDF: {ex.Message}");
                //    }
                //}
                report.Export(new PDFSimpleExport(), downloadsPath);
                try
                {
                    sendfile = ReadFile(downloadsPath);
                }
                catch
                {
                    sendfile = null;
                }
            }
            catch (Exception ex)
            {
                ErrorStr = ex.Message;
                //MessageBox.Show(ex.Message);
            }

        }
        public static byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes to read from file.
            //In this case we want to read entire file. So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);


            return data;
        }
        public static string PersianNumbersToEnglish(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] persian = "۰۱۲۳۴۵۶۷۸۹".ToCharArray();
            char[] english = "0123456789".ToCharArray();

            foreach (char c in persian)
            {
                input = input.Replace(c, english[Array.IndexOf(persian, c)]);
            }

            return input;
        }

        public byte[] sendfile;
        public DataTable GetSignerForm(Report report, int? formtypeid, string formid)
        {
            DataTable DtSigns = new DataTable();
            DataTable dtremindnew = new DataTable();
            using (SqlCommand _command2 = new SqlCommand(@"select USR_sign,dbo.mtsh(IS_SignDate) SignDate,PRG_Users.Name from [dbo].[PRG_InsertSigns]
join PRG_Users on USR_id=IS_usr_id  where  IS_formtypeid=" + formtypeid.ToString() + " and IS_formid=" + formid + @" and [USR_sign] is not null
order by IS_SignDate", cp.connect()))
            {
                _command2.CommandType = CommandType.Text;
                //_command2.Parameters.AddWithValue("@Id", FormID);
                //_command.Parameters.AddWithValue("@FormId", Formid);

                using (SqlDataAdapter adapter1 = new SqlDataAdapter(_command2))
                {
                    adapter1.Fill(dtremindnew);
                }
            }
            int rowsid = -1;
            foreach (DataRow dr in dtremindnew.Rows)
            {
                rowsid++;
                DtSigns.Columns.Add("Sign" + rowsid.ToString(), typeof(Byte[]));
            }

            if (rowsid == -1)
            {
                DtSigns.Columns.Add("Sign0", typeof(Byte[]));
                DtSigns.Columns.Add("Sign1", typeof(Byte[]));
                DtSigns.Columns.Add("Sign2", typeof(Byte[]));
                DtSigns.Columns.Add("Sign3", typeof(Byte[]));
                DtSigns.Columns.Add("Sign4", typeof(Byte[]));
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            if (rowsid == 0)
            {
                DtSigns.Columns.Add("Sign1", typeof(Byte[]));
                DtSigns.Columns.Add("Sign2", typeof(Byte[]));
                DtSigns.Columns.Add("Sign3", typeof(Byte[]));
                DtSigns.Columns.Add("Sign4", typeof(Byte[]));
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 1)
            {
                DtSigns.Columns.Add("Sign2", typeof(Byte[]));
                DtSigns.Columns.Add("Sign3", typeof(Byte[]));
                DtSigns.Columns.Add("Sign4", typeof(Byte[]));
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));

            }
            else if (rowsid == 2)
            {
                DtSigns.Columns.Add("Sign3", typeof(Byte[]));
                DtSigns.Columns.Add("Sign4", typeof(Byte[]));
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 3)
            {
                DtSigns.Columns.Add("Sign4", typeof(Byte[]));
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 4)
            {
                DtSigns.Columns.Add("Sign5", typeof(Byte[]));
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 5)
            {
                DtSigns.Columns.Add("Sign6", typeof(Byte[]));
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 6)
            {
                DtSigns.Columns.Add("Sign7", typeof(Byte[]));
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 7)
            {
                DtSigns.Columns.Add("Sign8", typeof(Byte[]));
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }
            else if (rowsid == 8)
            {
                DtSigns.Columns.Add("Sign9", typeof(Byte[]));
            }

            rowsid = -1;
            foreach (DataRow dr in dtremindnew.Rows)
            {
                rowsid++;
                DtSigns.Columns.Add("Sign" + rowsid.ToString() + "Date", typeof(string));
            }
            if (rowsid == -1)
            {
                DtSigns.Columns.Add("Sign0Date", typeof(string));
                DtSigns.Columns.Add("Sign1Date", typeof(string));
                DtSigns.Columns.Add("Sign2Date", typeof(string));
                DtSigns.Columns.Add("Sign3Date", typeof(string));
                DtSigns.Columns.Add("Sign4Date", typeof(string));
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            if (rowsid == 0)
            {
                DtSigns.Columns.Add("Sign1Date", typeof(string));
                DtSigns.Columns.Add("Sign2Date", typeof(string));
                DtSigns.Columns.Add("Sign3Date", typeof(string));
                DtSigns.Columns.Add("Sign4Date", typeof(string));
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 1)
            {
                DtSigns.Columns.Add("Sign2Date", typeof(string));
                DtSigns.Columns.Add("Sign3Date", typeof(string));
                DtSigns.Columns.Add("Sign4Date", typeof(string));
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 2)
            {
                DtSigns.Columns.Add("Sign3Date", typeof(string));
                DtSigns.Columns.Add("Sign4Date", typeof(string));
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 3)
            {
                DtSigns.Columns.Add("Sign4Date", typeof(string));
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 4)
            {
                DtSigns.Columns.Add("Sign5Date", typeof(string));
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 5)
            {
                DtSigns.Columns.Add("Sign6Date", typeof(string));
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 6)
            {
                DtSigns.Columns.Add("Sign7Date", typeof(string));
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 7)
            {
                DtSigns.Columns.Add("Sign8Date", typeof(string));
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }
            else if (rowsid == 8)
            {
                DtSigns.Columns.Add("Sign9Date", typeof(string));
            }

            rowsid = -1;
            foreach (DataRow dr in dtremindnew.Rows)
            {
                rowsid++;
                DtSigns.Columns.Add("Sign" + rowsid.ToString() + "Signer", typeof(string));
            }
            if (rowsid == -1)
            {
                DtSigns.Columns.Add("Sign0Signer", typeof(string));
                DtSigns.Columns.Add("Sign1Signer", typeof(string));
                DtSigns.Columns.Add("Sign2Signer", typeof(string));
                DtSigns.Columns.Add("Sign3Signer", typeof(string));
                DtSigns.Columns.Add("Sign4Signer", typeof(string));
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            if (rowsid == 0)
            {
                DtSigns.Columns.Add("Sign1Signer", typeof(string));
                DtSigns.Columns.Add("Sign2Signer", typeof(string));
                DtSigns.Columns.Add("Sign3Signer", typeof(string));
                DtSigns.Columns.Add("Sign4Signer", typeof(string));
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            else if (rowsid == 1)
            {
                DtSigns.Columns.Add("Sign2Signer", typeof(string));
                DtSigns.Columns.Add("Sign3Signer", typeof(string));
                DtSigns.Columns.Add("Sign4Signer", typeof(string));
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));

            }
            else if (rowsid == 2)
            {
                DtSigns.Columns.Add("Sign3Signer", typeof(string));
                DtSigns.Columns.Add("Sign4Signer", typeof(string));
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));

            }
            else if (rowsid == 3)
            {
                DtSigns.Columns.Add("Sign4Signer", typeof(string));
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));

            }
            else if (rowsid == 4)
            {
                DtSigns.Columns.Add("Sign5Signer", typeof(string));
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));

            }
            else if (rowsid == 5)
            {
                DtSigns.Columns.Add("Sign6Signer", typeof(string));
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            else if (rowsid == 6)
            {
                DtSigns.Columns.Add("Sign7Signer", typeof(string));
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            else if (rowsid == 7)
            {
                DtSigns.Columns.Add("Sign8Signer", typeof(string));
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            else if (rowsid == 8)
            {
                DtSigns.Columns.Add("Sign9Signer", typeof(string));
            }
            DtSigns.Rows.Add();
            rowsid = 0;
            foreach (DataRow dr in dtremindnew.Rows)
            {
                DtSigns.Rows[0][rowsid] = dr["USR_sign"];
                DtSigns.Rows[0][10 + rowsid] = dr["SignDate"].ToString();
                DtSigns.Rows[0][20 + rowsid] = dr["Name"].ToString();
                rowsid++;
            }
            return DtSigns;
        }

        public string[] yekan = new string[10] { "صفر", "يک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه" };
        public string[] dahgan = new string[10] { "", "", "بيست", "سي", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        public string[] dahyek = new string[10] { "ده", "يازده", "دوازده", "سيزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        public string[] sadgan = new string[10] { "", "يکصد", "دويست", "سيصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        public string[] basex = new string[5] { "", "هزار", "ميليون", "ميليارد", "تريليون" };

        public string num2sr(string snum)
        {
            string[] sum2 = snum.Split('/');
            if (sum2.Length == 1)
                sum2 = snum.Split('.');

            snum = sum2[0];

            //snum = decimal.Parse(snum).ToString();
            string stotal = "";
            int index = snum.IndexOf('ر');
            if (index >= 0)
                snum = snum.Remove(index);
            string[] num = snum.Split(',');
            snum = "";
            try
            {
                for (int j = 0; j < num.Length; j++)
                    snum += num[j];
                if (snum != "")
                {
                    if (snum == "0")
                    {
                        return yekan[0];
                    }
                    else
                    {
                        snum = snum.PadLeft(((snum.Length - 1) / 3 + 1) * 3, '0');
                        int L = snum.Length / 3 - 1;
                        for (int i = 0; i <= L; i++)
                        {
                            int b = int.Parse(snum.Substring(i * 3, 3));
                            if (b != 0)
                                stotal = stotal + getnum3(b) + " " + basex[L - i] + " و ";
                        }
                        stotal = stotal.Substring(0, stotal.Length - 3);
                    }
                }
            }
            catch
            {
            }
            return stotal;
        }
        public string getnum3(int num3)
        {
            string s = "";
            int d3, d12;
            d12 = num3 % 100;
            d3 = num3 / 100;
            if (d3 != 0)
                s = sadgan[d3] + " و ";
            if ((d12 >= 10) && (d12 <= 19))
            {
                s = s + dahyek[d12 - 10];
            }
            else
            {
                int d2 = d12 / 10;
                if (d2 != 0)
                    s = s + dahgan[d2] + " و ";
                int d1 = d12 % 10;
                if (d1 != 0)
                    s = s + yekan[d1] + " و ";
                s = s.Substring(0, s.Length - 3);
            }
            ;
            return s;
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/Quotations")]
        public string AddQuotations([FromBody] crmQuotationsRequestModelView request)
        {
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            try
            {
                if (request.Date == default(DateTime))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "تاریخ را وارد نمایید");
                }
                else if (string.IsNullOrWhiteSpace(request.Address))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "جزئیات آدرس محل تحویل را وارد نمایید");
                }
                else if (request.Location <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شهر محل تحویل را مشخص نمایید");
                }
                else if (request.SalesExpert <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نماینده فروش را مشخص نمایید");
                }
                else if (request.TypeSale < 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نوع فروش را مشخص نمایید");
                }
                else if (request.Items == null)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ارسال مشخصات اقلام الزامی میباشد");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "داده های ارسال شده ناقص میباشد");
            }
            DateTime Date = request.Date;
            DateTime expire_date = Date.AddDays(clsProperties.GetPropertise("expireddayQuotation").ToInt());
            int TypeSale = request.TypeSale;
            if (TypeSale == 1)
                TypeSale = 0;
            else if (TypeSale == 2)
                TypeSale = 1;
            else if (TypeSale == 3)
                TypeSale = 2;

            string Description = request.Description;
            int Location = request.Location;
            string Address = request.Address;
            int SalesExpert = request.SalesExpert;
            DataTable header = new DataTable();
            header.Columns.Add("CustomerId", typeof(int));
            header.Columns.Add("SalesExpert", typeof(int));
            header.Columns.Add("Date", typeof(DateTime));
            header.Columns.Add("expire_date", typeof(DateTime));
            header.Columns.Add("TypeSale", typeof(int));
            header.Columns.Add("Description", typeof(string));
            header.Columns.Add("Location", typeof(int));
            header.Columns.Add("Address", typeof(string));
            header.Rows.Add(UserId, SalesExpert, Date, expire_date, TypeSale, Description, Location, Address);
            DataTable listitem = new DataTable();
            listitem.Columns.Add("is_id", typeof(int));
            listitem.Columns.Add("stock_id", typeof(int));
            listitem.Columns.Add("itm_id", typeof(int));
            listitem.Columns.Add("unit1_id", typeof(int));
            listitem.Columns.Add("unit2_id", typeof(int));
            listitem.Columns.Add("value", typeof(double));
            listitem.Columns.Add("valueSecendry", typeof(double));
            listitem.Columns.Add("fee", typeof(decimal));
            listitem.Columns.Add("price", typeof(decimal));
            listitem.Columns.Add("netPrice", typeof(decimal));
            listitem.Columns.Add("tax", typeof(int));
            listitem.Columns.Add("duty", typeof(int));
            listitem.Columns.Add("discountCustomer", typeof(double));
            listitem.Columns.Add("nesbat", typeof(bool));
            listitem.Columns.Add("nesbatValue", typeof(float));
            bool itemError = true;
            foreach (crmQuotationsRequestModelView.Item items in request.Items)
            {
                if (items.IS_id <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.value <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.valueSecondary < 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.fi <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.Price <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.NetPrice <= 0)
                {
                    itemError = false;
                    break;
                }
                DataTable item_info = cp.select_row("API_GetIteminfoByItemStock",
                    new SqlParameter[] { new SqlParameter("is_id", items.IS_id), new SqlParameter("user_id", UserId) });
                DataRow dr = item_info.Rows[0];
                listitem.Rows.Add(items.IS_id, dr["stock_id"], dr["itm_id"], dr["unit1_id"], dr["unit2_id"], items.value, items.valueSecondary, items.fi, items.Price, items.NetPrice, dr["tax"], dr["dury"], dr["discountCustomer"], dr["nesbat"], dr["nesbatValue"]);
            }

            if (itemError == false)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "داده های ارسال شده در اقلام کالا ناقص میباشد");
            }
            DataRow dr_header = header.Rows[0];
            clsQuotations qu = new clsQuotations();
            if (qu.Etebar(dr_header, listitem))
            {
                if (qu.inventory(dr_header, listitem))
                {
                    int scaler = qu.Acsept(dr_header, listitem);
                    if (scaler >= 1)
                    {
                        response_quotations = new crmQuotationsResponseModelView(scaler);
                        return JsonConvert.SerializeObject(response_quotations);
                    }
                    else
                    {
                        return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), qu.MessageError);
                    }
                }
                else
                {
                    return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), qu.MessageError);
                }
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), qu.MessageError);
            }

            return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "");
        }

        [Authorize]
        [HttpPost]
        [Route("api/[controller]/RequestBuys")]
        public string AddRequestBuys([FromBody] crmRequestBuysRequestModelView request)
        {
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            try
            {
                if (request.Date == default(DateTime))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "تاریخ را وارد نمایید");
                }
                else if (string.IsNullOrWhiteSpace(request.Address))
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "جزئیات آدرس محل تحویل را وارد نمایید");
                }
                else if (request.Location <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شهر محل تحویل را مشخص نمایید");
                }
                else if (request.SalesExpert <= 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نماینده فروش را مشخص نمایید");
                }
                else if (request.TypeSale < 0)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "نوع فروش را مشخص نمایید");
                }
                else if (request.Items == null)
                {
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ارسال مشخصات اقلام الزامی میباشد");
                }
            }
            catch
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "داده های ارسال شده ناقص میباشد");
            }
            DateTime Date = request.Date;
            int TypeSale = request.TypeSale;
            string Description = request.Description;
            int Location = request.Location;
            string Address = request.Address;
            int SalesExpert = request.SalesExpert;
            DataTable header = new DataTable();
            header.Columns.Add("CustomerId", typeof(int));
            header.Columns.Add("SalesExpert", typeof(int));
            header.Columns.Add("Date", typeof(DateTime));
            header.Columns.Add("TypeSale", typeof(int));
            header.Columns.Add("Description", typeof(string));
            header.Columns.Add("Location", typeof(int));
            header.Columns.Add("Address", typeof(string));
            header.Rows.Add(UserId, SalesExpert, Date, TypeSale, Description, Location, Address);
            DataTable listitem = new DataTable();
            listitem.Columns.Add("is_id", typeof(int));
            listitem.Columns.Add("stock_id", typeof(int));
            listitem.Columns.Add("itm_id", typeof(int));
            listitem.Columns.Add("unit1_id", typeof(int));
            listitem.Columns.Add("unit2_id", typeof(int));
            listitem.Columns.Add("value", typeof(double));
            listitem.Columns.Add("valueSecendry", typeof(double));
            listitem.Columns.Add("nesbat", typeof(bool));
            listitem.Columns.Add("nesbatValue", typeof(float));
            bool itemError = true;
            foreach (crmRequestBuysRequestModelView.ItemReq items in request.Items)
            {
                if (items.IS_id <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.value <= 0)
                {
                    itemError = false;
                    break;
                }
                else if (items.valueSecondary < 0)
                {
                    itemError = false;
                    break;
                }
                DataTable item_info = cp.select_row("API_GetIteminfoByItemStock",
                    new SqlParameter[] { new SqlParameter("is_id", items.IS_id), new SqlParameter("user_id", UserId) });
                DataRow dr = item_info.Rows[0];
                listitem.Rows.Add(items.IS_id, dr["stock_id"], dr["itm_id"], dr["unit1_id"], dr["unit2_id"], items.value, items.valueSecondary, dr["nesbat"], dr["nesbatValue"]);
            }

            if (itemError == false)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "داده های ارسال شده در اقلام کالا ناقص میباشد");
            }
            DataRow dr_header = header.Rows[0];
            clsRequestBuys Re = new clsRequestBuys();
            if (Re.Etebar(dr_header, listitem))
            {
                if (Re.inventory(dr_header, listitem))
                {
                    int scaler = Re.Acsept(dr_header, listitem);
                    if (scaler >= 1)
                    {
                        response_quotations = new crmQuotationsResponseModelView(scaler);
                        return JsonConvert.SerializeObject(response_quotations);
                    }
                    else
                    {
                        return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), Re.MessageError);
                    }
                }
                else
                {
                    return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), Re.MessageError);
                }
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), Re.MessageError);
            }

            return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "");
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GetRelationForm/{FormID}/{FormType}")]
        public string GetRelationForm(int? FormID, string? FormType)
        {
            if (FormType == "0")
                FormType = "(N'Quotation','QuotationLadingStock')";
            else if (FormType == "1")
                FormType = "(N'Lading')";

            if (FormID == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Formid ارائه نشده است.");
            }
            ListResponse list = new ListResponse();
            List<CrmGetRelationFormDetailModel> Relation = new List<CrmGetRelationFormDetailModel>();
            messagebase m = new messagebase();
            //int UserId = HttpContext.Items["UserId"].ToInt();
            DataTable dt_RelationForm = new DataTable();
            using (SqlCommand _command = new SqlCommand("API_GetRelationForm", cp.connect()))
            {
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@type", 0);
                _command.Parameters.AddWithValue("@FormId", FormID);
                _command.Parameters.AddWithValue("@FormType", FormType);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_RelationForm);
                }
            }
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_RelationForm.Rows.Count <= 0 & cls_public.sqlError.message != null)
            {
                return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_RelationForm.Rows)
                {
                    Relation.Add(new CrmGetRelationFormDetailModel()
                    {
                        Senddate = row["RFF_Senddate"].ToString(),
                        TypeReffer = row["LKP_name"].ToString(),
                        Sender = row["sender"].ToString(), // چون nullable هست
                        Subject = row["RFF_Subject"].ToString(),
                        typeChart = row["RFF_Summary"].ToString(),
                        Description = row["RFF_Description"].ToString(),
                        Receiver = row["reciever"].ToString(),
                        Showdate = row["RFF_Showdate"].ToString(),
                        Confirmer = row["confier"].ToString(),
                        Confirmdate = row["RFF_confirmdate"].ToString(),
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = Relation
                };

            }
            if (Relation.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = new List<CrmGetRelationFormModel>()
                };
            }
            response_quotations = new crmQuotationsResponseModelView(list);
            return JsonConvert.SerializeObject(response_quotations);
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GetQuotationsRelate/{Formid}")]
        public string GetQuotationsRelate(int? Formid)
        {
            if (Formid == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Formid ارائه نشده است.");
            }
            ListResponse list = new ListResponse();
            List<CrmGetRelationFormModel> Quotations = new List<CrmGetRelationFormModel>();
            messagebase m = new messagebase();
            //int UserId = HttpContext.Items["UserId"].ToInt();
            DataTable dt_Quotations = new DataTable();
            using (SqlCommand _command = new SqlCommand("API_GetInformationRelate", cp.connect()))
            {
                _command.CommandType = CommandType.StoredProcedure;

                _command.Parameters.AddWithValue("@type", 0);
                _command.Parameters.AddWithValue("@FormId", Formid);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_Quotations);
                }
            }
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0 & cls_public.sqlError.message != null)
            {
                return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Quotations.Rows)
                {
                    Quotations.Add(new CrmGetRelationFormModel()
                    {
                        ID = row["ID"].ToInt(),
                        DateForm = row["DateForm"].ToString(),
                        Number = row["Number"].ToString(), // چون nullable هست
                        CycleFormDetailState = row["CycleFormDetailState"].ToString(),
                        valueform = row["valueform"].ToDecimal(),
                        NetPriceform = row["NetPriceform"].ToDecimal(),
                        SalesExpertform = row["SalesExpertform"].ToString(),
                        SalesExpertId = row["SalesExpertId"].ToString(),
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = Quotations
                };

            }
            if (Quotations.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = new List<CrmGetRelationFormModel>()
                };
            }
            response_quotations = new crmQuotationsResponseModelView(list);
            return JsonConvert.SerializeObject(response_quotations);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GetLadingRelate/{Formid}")]
        public string GetLadingRelate(int? Formid)
        {
            if (Formid == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Formid ارائه نشده است.");
            }
            ListResponse list = new ListResponse();
            List<CrmGetRelationFormModel> Quotations = new List<CrmGetRelationFormModel>();
            messagebase m = new messagebase();
            //int UserId = HttpContext.Items["UserId"].ToInt();
            DataTable dt_Quotations = new DataTable();
            using (SqlCommand _command = new SqlCommand("API_GetInformationRelate", cp.connect()))
            {
                _command.CommandType = CommandType.StoredProcedure;

                _command.Parameters.AddWithValue("@type", 1);
                _command.Parameters.AddWithValue("@FormId", Formid);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_Quotations);
                }
            }
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0 & cls_public.sqlError.message != null)
            {
                return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                foreach (DataRow row in dt_Quotations.Rows)
                {
                    Quotations.Add(new CrmGetRelationFormModel()
                    {
                        ID = row["ID"].ToInt(),
                        DateForm = row["DateForm"].ToString(),
                        Number = row["Number"].ToString(), // چون nullable هست
                        CycleFormDetailState = row["CycleFormDetailState"].ToString(),
                        valueform = row["valueform"].ToDecimal(),
                        NetPriceform = row["NetPriceform"].ToDecimal(),
                        SalesExpertform = row["SalesExpertform"].ToString(),
                        SalesExpertId = row["SalesExpertId"].ToString(),
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = Quotations
                };

            }
            if (Quotations.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = new List<CrmGetRelationFormModel>()
                };
            }
            response_quotations = new crmQuotationsResponseModelView(list);
            return JsonConvert.SerializeObject(response_quotations);
        }
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/GetQuotationsList/{Period}")]
        public string GetQuotationsList(int? Period)//[FromQuery] ListRequest request)
        {
            //if (request.page_number < 1)
            //{
            //    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            //}
            //int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmQuotationsResponseModel> Quotations = new List<crmQuotationsResponseModel>();
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            //int Period = HttpContext.Items["Period"].ToInt();
            DateTime dtyears = DateTime.Now;
            if (Period == 0)
                dtyears = cp.GetDateTime().AddMonths(-1);
            else if (Period == 1)
                dtyears = cp.GetDateTime().AddMonths(-3);
            else if (Period == 2)
                dtyears = cp.GetDateTime().AddMonths(-6);
            else if (Period == 3)
            {
                DateTime dt1 = ShamsiDate.Shamsi2Miladi(ShamsiDate.MiladiToShamsi(cp.GetDateTimeInvariantCulture()));
                DateTime dt2 = ShamsiDate.Shamsi2Miladi(cp.GetDateTime().Year.ToString() + "/01/01");
                TimeSpan diff = dt1 - dt2;
                //dtyears = ShamsiDate.Shamsi2Miladi(cp.GetDateTimeInvariantCulture().Year.ToString() + "/01/01");
                dtyears = cp.GetDateTimeInvariantCulture().AddDays(-diff.Days);
            }
            DataTable dt_Quotations = new DataTable();
            using (SqlCommand _command = new SqlCommand("API_ListQuotations", cp.connect()))
            {
                _command.CommandType = CommandType.StoredProcedure;
                _command.Parameters.AddWithValue("@Customer_id", UserId);
                _command.Parameters.AddWithValue("@FormId", 0);
                _command.Parameters.AddWithValue("@date", dtyears.Date);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_Quotations);
                }
            }
            //SqlParameter[] parameters = new SqlParameter[]
            //{
            //        new SqlParameter("@Customer_id",UserId),
            //                            new SqlParameter("@FormId",0),
            //new SqlParameter("@Period",Period)
            //};
            //dt_Quotations = cp.select_row("API_ListQuotations", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0 & cls_public.sqlError.message != null)
            {
                return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                //Pages = dt_Quotations.Rows.Count / 10;
                //if (Pages < 1)
                //    Pages = 1;
                //if (dt_Quotations.Rows.Count > 10)
                //{
                //    dt_Quotations = dt_Quotations.AsEnumerable().Skip((request.page_number - 1) * 10).CopyToDataTable();
                //    dt_Quotations = dt_Quotations.AsEnumerable().Take(10).CopyToDataTable();
                //}
                //Quotations = new crmQuotationsResponseModel();
                foreach (DataRow row in dt_Quotations.Rows)
                {
                    //crmQuotationsResponseModel.Cycle cycle = new crmQuotationsResponseModel.Cycle()
                    //{
                    //    //RB_id = string.IsNullOrEmpty(row["RB_id"].ToString()) ? null : row["RB_id"].ToInt(),
                    //    //RB_status = string.IsNullOrEmpty(row["RB_Status"].ToString()) ? null : row["RB_Status"].ToInt(),

                    //    QU_id = string.IsNullOrEmpty(row["QU_id"].ToString()) ? null : row["QU_id"].ToInt(),
                    //    QU_status = string.IsNullOrEmpty(row["QU_Status"].ToString()) ? null : row["QU_Status"].ToInt(),

                    //    INV_id = string.IsNullOrEmpty(row["INV_id"].ToString()) ? null : row["INV_id"].ToInt(),
                    //    INV_status = string.IsNullOrEmpty(row["INV_Status"].ToString()) ? null : row["INV_Status"].ToInt(),

                    //    LD_id = string.IsNullOrEmpty(row["LD_id"].ToString()) ? null : row["LD_id"].ToInt(),
                    //    LD_status = string.IsNullOrEmpty(row["LD_Status"].ToString()) ? null : row["LD_Status"].ToInt(),
                    //};
                    //Quotations = new crmQuotationsResponseModel()
                    //
                    Quotations.Add(new crmQuotationsResponseModel()
                    {
                        FormID = row["FormID"].ToInt(),
                        Pm = row["Pm"].ToBoolean(),
                        StatePay = row["StatePay"].ToString(), // چون nullable هست
                        TypeRequest = row["TypeRequest"].ToBoolean(),
                        dateForm = row["dateForm"].ToString(),
                        NumberForm = row["NumberForm"].ToString(),
                        TypeSale = row["TypeSale"].ToString(),
                        Price = row["Price"].ToDecimal(),
                        Discount = row["Discount"].ToDecimal(),
                        Tax = row["Tax"].ToDecimal(),
                        NetPrice = row["NetPrice"].ToDecimal(),
                        TypeForm = row["TypeForm"].ToInt(),
                        Portal = row["Portal"].ToBoolean(),
                        //CycleStatus = cycle,
                        Details = Array.Empty<crmQuotationsResponseModel.Detail>() // فعلاً خالی
                    });
                }
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = Quotations
                };

            }
            if (Quotations.Count <= 0)
            {
                list = new ListResponse()
                {
                    Pages = 1,
                    Page_row = 1,//request.page_number,
                    List = new List<crmQuotationsResponseModel>()
                };
            }
            //response_quotations = new crmQuotationsResponseModelView(Quotations.ToArray());
            //response_quotations = new crmQuotationsResponseModelView(Quotations);
            response_quotations = new crmQuotationsResponseModelView(list);
            return JsonConvert.SerializeObject(response_quotations);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Quotation/{QuotationId}")]
        public string GetQuotations(int? QuotationId)
        {
            if (QuotationId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "QuotationId ارائه نشده است.");
            }
            crmQuotationsResponseModel Quotations = null;
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            DataTable dt_Quotations = new DataTable();
            DateTime dtyears = ShamsiDate.Shamsi2Miladi(cp.GetDateTime().Year.ToString() + "/01/01");

            using (SqlCommand _command = new SqlCommand("API_ListQuotations", cp.connect()))
            {
                _command.CommandType = CommandType.StoredProcedure;

                _command.Parameters.AddWithValue("@Customer_id", UserId);
                _command.Parameters.AddWithValue("@FormId", QuotationId);
                _command.Parameters.AddWithValue("@date", dtyears.Date);

                using (SqlDataAdapter adapter = new SqlDataAdapter(_command))
                {
                    adapter.Fill(dt_Quotations);
                }
            }
            //SqlParameter[] parameters = new SqlParameter[]
            //{
            //        new SqlParameter("Customer_id",UserId),
            //        new SqlParameter("Period",3),
            //        new SqlParameter("QuotationId",QuotationId)
            //};
            //DataTable dt_Quotations = new DataTable();
            //dt_Quotations = cp.select_row("API_ListQuotations", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                //DataView dv = new DataView(dt_Quotations);
                //dv.RowFilter = $"QU_id = {QuotationId}";
                //dt_Quotations = dv.ToTable();
                if (dt_Quotations.Rows.Count > 0)
                {
                    DataTable dt_QuItems = cp.select_row("API_QuotationItems",
                        new SqlParameter[] { new SqlParameter("QuotationId", QuotationId) });
                    List<crmQuotationsResponseModel.Detail> details = new List<crmQuotationsResponseModel.Detail>();
                    foreach (DataRow row in dt_QuItems.Rows)
                    {
                        details.Add(new crmQuotationsResponseModel.Detail
                        {
                            id = row["QUI_id"].ToInt(),
                            Code = row["Code"].ToString(),
                            StockTitle = row["stocktitle"].ToString(),
                            Title = row["Title"].ToString(),
                            SupplierId = row["ITM_Supplier"].ToInt(),
                            SupplierName = row["Supplier"].ToString(),
                            Unit1 = row["vahed"].ToString(),
                            value = row["QUI_value"].ToDouble(),
                            valueSecondary = row["QUI_valueSecondary"].ToDouble(),
                            fee = row["QUI_fee"].ToDecimal(),
                            Price = row["QUI_Price"].ToDecimal(),
                            Discount = row["QUI_Discount"].ToDecimal(),
                            DistinctPercent = row["DistinctPercent"].ToDouble(),
                            Tax = row["tax"].ToInt(),
                            Duty = row["duty"].ToInt(),
                            Addition = row["addition"].ToDecimal(),
                            NetPrice = row["QUI_NetPrice"].ToDecimal(),
                            Unit2 = row["unit2"].ToString(),
                            Nesabt = row["nesabt"].ToBoolean(),
                            NesbatValue = row["nesbatValue"].ToDouble(),
                            Item_tax = row["ITM_tax"].ToInt(),
                            Item_duty = row["ITM_duty"].ToInt(),
                            Item_id = row["QUI_it_id"].ToInt(),
                            StockId = row["QUI_stock"].ToInt(),
                            DistinctCustomersBrand = row["DistinctCustomersBrand"].ToDouble(),
                            ItemStock_Id = row["IS_id"].ToInt(),
                            ItemBrand = row["itembrand"].ToString()
                        });
                    }

                    //crmQuotationsResponseModel.Cycle cycle = new crmQuotationsResponseModel.Cycle()
                    //{
                    //    RB_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["RB_Status"].ToString()) ? null : dt_Quotations.Rows[0]["RB_Status"].ToInt(),
                    //    QU_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["QU_Status"].ToString()) ? null : dt_Quotations.Rows[0]["QU_Status"].ToInt(),
                    //    INV_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["INV_Status"].ToString()) ? null : dt_Quotations.Rows[0]["INV_Status"].ToInt(),
                    //};

                    Quotations = new crmQuotationsResponseModel()
                    {
                        FormID = dt_Quotations.Rows[0]["FormID"].ToInt(),
                        Pm = dt_Quotations.Rows[0]["Pm"].ToBoolean(),
                        StatePay = dt_Quotations.Rows[0]["StatePay"].ToString(), // چون nullable هست
                        TypeRequest = dt_Quotations.Rows[0]["TypeRequest"].ToBoolean(),
                        dateForm = dt_Quotations.Rows[0]["dateForm"].ToString(),
                        NumberForm = dt_Quotations.Rows[0]["NumberForm"].ToString(),
                        TypeSale = dt_Quotations.Rows[0]["TypeSale"].ToString(),
                        Price = dt_Quotations.Rows[0]["Price"].ToDecimal(),
                        Discount = dt_Quotations.Rows[0]["Discount"].ToDecimal(),
                        Tax = dt_Quotations.Rows[0]["Tax"].ToDecimal(),
                        NetPrice = dt_Quotations.Rows[0]["NetPrice"].ToDecimal(),
                        TypeForm = dt_Quotations.Rows[0]["TypeForm"].ToInt(),
                        Portal = dt_Quotations.Rows[0]["Portal"].ToBoolean(),
                        //CycleStatus = cycle,
                        Details = details.ToArray() // فعلاً خالی
                    };
                }

            }
            response_quotations = new crmQuotationsResponseModelView(Quotations);

            return JsonConvert.SerializeObject(response_quotations);
        }
        //public string GetRequestBuy(int? RequestBuyId)
        //{
        //    if (RequestBuyId == null)
        //    {
        //        return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "RequestBuyId ارائه نشده است.");
        //    }
        //    crmQuotationsResponseModel RequestBuys = null;
        //    messagebase m = new messagebase();
        //    int UserId = HttpContext.Items["UserId"].ToInt();
        //    SqlParameter[] parameters = new SqlParameter[]
        //    {
        //            new SqlParameter("Customer_id",UserId)
        //    };
        //    DataTable dt_RequestBuy = new DataTable();
        //    dt_RequestBuy = cp.select_row("API_ListQuotations", parameters);
        //    if (cls_public.sqlError.success == false)
        //    {
        //        return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
        //    }
        //    if (dt_RequestBuy.Rows.Count <= 0)
        //    {
        //        if (cls_public.sqlError.message != null)
        //            return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
        //    }
        //    else
        //    {
        //        DataView dv = new DataView(dt_RequestBuy);
        //        dv.RowFilter = $"QU_id = {RequestBuyId}";
        //        dt_RequestBuy = dv.ToTable();
        //        if (dt_RequestBuy.Rows.Count > 0)
        //        {
        //            DataTable dt_REItems = cp.select_row("API_QuotationItems",
        //                new SqlParameter[] { new SqlParameter("RequestBuyId", RequestBuyId) });
        //            List<crmQuotationsResponseModel.Detail> details = new List<crmQuotationsResponseModel.Detail>();
        //            foreach (DataRow row in dt_REItems.Rows)
        //            {
        //                details.Add(new crmQuotationsResponseModel.Detail
        //                {
        //                    id = row["QUI_id"].ToInt(),
        //                    Code = row["Code"].ToString(),
        //                    StockTitle = row["stocktitle"].ToString(),
        //                    Title = row["Title"].ToString(),
        //                    SupplierId = row["ITM_Supplier"].ToInt(),
        //                    SupplierName = row["Supplier"].ToString(),
        //                    Unit1 = row["vahed"].ToString(),
        //                    value = row["QUI_value"].ToDouble(),
        //                    valueSecondary = row["QUI_valueSecondary"].ToDouble(),
        //                    fee = row["QUI_fee"].ToDecimal(),
        //                    Price = row["QUI_Price"].ToDecimal(),
        //                    Discount = row["QUI_Discount"].ToDecimal(),
        //                    DistinctPercent = row["DistinctPercent"].ToDouble(),
        //                    Tax = row["tax"].ToInt(),
        //                    Duty = row["duty"].ToInt(),
        //                    Addition = row["addition"].ToDecimal(),
        //                    NetPrice = row["QUI_NetPrice"].ToDecimal(),
        //                    Unit2 = row["unit2"].ToString(),
        //                    Nesabt = row["nesabt"].ToBoolean(),
        //                    NesbatValue = row["nesbatValue"].ToDouble(),
        //                    Item_tax = row["ITM_tax"].ToInt(),
        //                    Item_duty = row["ITM_duty"].ToInt(),
        //                    Item_id = row["QUI_it_id"].ToInt(),
        //                    StockId = row["QUI_stock"].ToInt(),
        //                    DistinctCustomersBrand = row["DistinctCustomersBrand"].ToDouble(),
        //                    ItemStock_Id = row["IS_id"].ToInt(),
        //                    ItemBrand = row["itembrand"].ToString()
        //                });
        //            }

        //            crmQuotationsResponseModel.Cycle cycle = new crmQuotationsResponseModel.Cycle()
        //            {
        //                RB_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["RB_Status"].ToString()) ? null : dt_Quotations.Rows[0]["RB_Status"].ToInt(),
        //                QU_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["QU_Status"].ToString()) ? null : dt_Quotations.Rows[0]["QU_Status"].ToInt(),
        //                INV_status = string.IsNullOrEmpty(dt_Quotations.Rows[0]["INV_Status"].ToString()) ? null : dt_Quotations.Rows[0]["INV_Status"].ToInt(),
        //            };

        //            Quotations = new crmQuotationsResponseModel()
        //            {
        //                QU_id = dt_Quotations.Rows[0]["QU_id"].ToInt(),
        //                QU_no = dt_Quotations.Rows[0]["QU_no"].ToInt(),
        //                RB_no = dt_Quotations.Rows[0]["RB_no"].ToInt(), // چون nullable هست
        //                QU_date = dt_Quotations.Rows[0]["dateQ"].ToString(),
        //                Customer = dt_Quotations.Rows[0]["CustomerName"].ToString(),
        //                Supplier = dt_Quotations.Rows[0]["TaminKonandeh"].ToString(),
        //                TypeSale = dt_Quotations.Rows[0]["TypeSale"].ToString(),
        //                TypePayment = dt_Quotations.Rows[0]["how_pay"].ToString(),
        //                NetPrice = dt_Quotations.Rows[0]["QU_NetPrice"].ToDecimal(),
        //                City = dt_Quotations.Rows[0]["city_name"].ToString(),
        //                value = dt_Quotations.Rows[0]["QUIvalue"].ToDecimal(),
        //                valueSecondary = dt_Quotations.Rows[0]["QUIvalueSecondary"].ToDecimal(),
        //                SalesExpert = dt_Quotations.Rows[0]["Karshenas"].ToString(),
        //                Status = dt_Quotations.Rows[0]["QUTypeTitle"].ToString(),
        //                StatusId = dt_Quotations.Rows[0]["QUType"].ToInt(),
        //                TypeSaleId = dt_Quotations.Rows[0]["QU_typesale"].ToInt(),
        //                DLCode = dt_Quotations.Rows[0]["DLCode"].ToInt(),
        //                SalesExpertId = dt_Quotations.Rows[0]["QU_SalesExpert"].ToInt(),
        //                CustomerId = dt_Quotations.Rows[0]["QU_cst_id"].ToInt(),
        //                Tell = dt_Quotations.Rows[0]["tel_tell"].ToString(),
        //                CycleStatus = cycle,
        //                Details = details.ToArray() // فعلاً خالی
        //            };
        //        }

        //    }
        //    response_quotations = new crmQuotationsResponseModelView(Quotations);

        //    return JsonConvert.SerializeObject(response_quotations);
        //}

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Cycle/{Id}")]
        public string GetCycle(int? Type, int? Id)
        {
            if (Type == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Type ارائه نشده است.");
            }

            if (Id == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "Id ارائه نشده است.");
            }

            List<crmCycleRefferingResponseModel> cycle = new List<crmCycleRefferingResponseModel>();
            messagebase m = new messagebase();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("FormId", Id),
                new SqlParameter("FormType", Type)
            };
            DataTable dt_cycle = new DataTable();
            dt_cycle = cp.select_row("API_GetCycleReffering", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            else
            {
                if (dt_cycle.Rows.Count > 0)
                {
                    int i = 1;
                    foreach (DataRow row in dt_cycle.Rows)
                    {
                        cycle.Add(new crmCycleRefferingResponseModel()
                        {
                            SendDate = string.IsNullOrEmpty(row["RFF_Senddate"].ToString()) ? null : Convert.ToDateTime(row["RFF_Senddate"]),
                            Sender = row["sender"].ToString(),
                            Reciever = row["reciever"].ToString(),
                            Subject = clsProperties.GetPropertise("Web_ShowSubjectCycle").ToBoolean() ? row["RFF_Subject"].ToString() : null,
                            Summary = row["RFF_Summary"].ToString(),
                            ShowDate = Convert.ToDateTime(row["RFF_Showdate"]),
                            Approver = row["confier"].ToString(),
                            ConfirmDate = string.IsNullOrEmpty(row["RFF_confirmdate"].ToString()) ? null : Convert.ToDateTime(row["RFF_confirmdate"].ToString()),
                            Valid_id = row["RFF_Valid"].ToBoolean(),
                            last_cycle = i == dt_cycle.Rows.Count ? true : false
                        });

                        i++;
                    }

                }

            }
            response_Cycle = new crmCycleRefferingResponseModelView(cycle);
            return JsonConvert.SerializeObject(response_Cycle);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/bill")] // صورت حساب مشتری
        public string GetBill([FromQuery] ListRequest request)
        {
            if (request.page_number < 1)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            }
            int UserId = HttpContext.Items["UserId"].ToInt();
            bool ShowBillTurnovers = false;
            SqlParameter[] parameters_user =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByIdCrm", parameters_user);
            if (dt_InfoCustomer.Rows.Count > 0)
            {
                //Classes.Library.WriteErrorLog(" dt_InfoCustomer " + dt_InfoCustomer.Rows.Count.ToString());
                ShowBillTurnovers = dt_InfoCustomer.Rows[0]["BillTurnovers"].ToBoolean();
            }

            int Pages = 1;
            try
            {

                string DataMali = clsProperties.GetPropertise("DataMali").ToString();
                DateTime DTNOW = cp.GetDateTime().Date;
                DateTime DTNOW2 = DTNOW;
                DataTable customerkoli2;

                string DTNOWStr = ShamsiDate.MiladiToShamsi(DTNOW);
                DTNOWStr = DTNOWStr.Substring(0, 4) + "/01/01";
                DTNOW = ShamsiDate.Shamsi2Miladi(DTNOWStr);
                string DTNOWStr2 = ShamsiDate.MiladiToShamsi(DTNOW2.Date);
                try
                {
                    string DTNOWStr3 = DTNOWStr2.Substring(0, 4) + "/12/30";
                    DTNOW2 = ShamsiDate.Shamsi2Miladi(DTNOWStr3);
                }
                catch
                {
                    string DTNOWStr3 = DTNOWStr2.Substring(0, 4) + "/12/29";
                    DTNOW2 = ShamsiDate.Shamsi2Miladi(DTNOWStr3);
                }
                //                string text = @"SELECT DLRef, DLCode, DLTitle,  SumDebit, SumCredit,DLType,
                //    CASE WHEN Balance > 0 THEN Balance ELSE 0 END DebitBalance,
                //    CASE WHEN Balance < 0 THEN ABS(Balance)	ELSE 0 END CreditBalance
                //FROM
                //    (
                //    SELECT DLRef, DLCode, DLTitle, DLTitle_En,DLType,
                //        ISNULL(SUM(VI.Debit), 0) SumDebit, ISNULL(SUM(VI.Credit), 0) SumCredit,
                //        ISNULL(SUM(VI.Debit), 0) - ISNULL(SUM(VI.Credit), 0) Balance,
                //        IsCustomer, IsVendor
                //    FROM " + DataMali + @".ACC.vwVoucherItem VI
                //    WHERE 
                //			(VI.DLRef IS NOT NULL)
                //        AND (VI.VoucherDate            >=  @FromDate   )
                //        AND (VI.VoucherDate            <=  @ToDate    )
                //        AND ((VI.VoucherType IN (1, 2, 6)) --General, OtherSystems, CurrencyConversion
                //            OR  (VoucherType =  N'5'       AND VoucherDate <= @LastDayOfFirstYear)
                //            OR  (VoucherType =  -1       AND VoucherDate >= @FirstDayOfLastYear)
                //            OR  (VoucherType =  -1)
                //            )
                //    GROUP BY DLRef, DLCode, DLTitle, DLTitle_En,DLType, IsCustomer, IsVendor   
                //having DLRef=@Dlref) AS Moror";

                //Classes.Library.WriteErrorLog(" before Sorathesab " );
                int checkmororhesab = 0;
                try
                {
                    checkmororhesab = clsProperties.GetPropertise("Sorathesab").ToInt();
                }
                catch { }
                //Classes.Library.WriteErrorLog(" checkmororhesab " + checkmororhesab.ToString() );
                DataTable dlref = cp.select_command("SELECT csT_dl FROM dbo.PRG_Customers WHERE cst_id = " + UserId);
                //Classes.Library.WriteErrorLog(" checkmororhesab " + dlref.Rows.Count.ToString());
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@type", false),
                new SqlParameter("@Dlref", dlref.Rows[0][0]),
                new SqlParameter("@FromDate", DTNOW),
                new SqlParameter("@ToDate", DTNOW2),
                new SqlParameter("@LastDayOfFirstYear",DTNOW),
                new SqlParameter("@FirstDayOfLastYear",DTNOW2 ),
                };
                DataTable dtmoror = cp.select_row("SpSelectMororHesabHeather", parameters);
                //Classes.Library.WriteErrorLog(" SpSelectMororHesabHeather " + dtmoror.Rows.Count.ToString());

                if (dtmoror.Rows.Count > 0)
                {

                    crmBillResponseModel header = new crmBillResponseModel()
                    {
                        Id = dtmoror.Rows[0]["DLRef"].ToInt(),
                        Code = dtmoror.Rows[0]["DLCode"].ToInt(),
                        Type = dtmoror.Rows[0]["DLType"].ToInt(),
                        Title = dtmoror.Rows[0]["DLTitle"].ToString(),
                        SumDebit = dtmoror.Rows[0]["SumDebit"].ToDecimal(),
                        SumCredit = dtmoror.Rows[0]["SumCredit"].ToDecimal(),
                        DebitBalance = dtmoror.Rows[0]["DebitBalance"].ToDecimal(),
                        CreditBalance = dtmoror.Rows[0]["CreditBalance"].ToDecimal()
                    };
                    List<crmBillResponseModel.turnover> turnover = new List<crmBillResponseModel.turnover>();
                    if (ShowBillTurnovers)
                    {

//                       string text = @"SELECT   [RowNumber] ,[VoucherDate] ,[Description] ,[Debit] ,[Credit]
//FROM " + DataMali + @".ACC.[vwVoucherItem] vi
//LEFT JOIN (SELECT    g.VoucherRef   ,g1.Number   ,g1.FromVoucherNumber   ,g1.ToVoucherNumber
//  FROM " + DataMali + @".ACC.vwGLVoucher g1
//  INNER JOIN " + DataMali + @".ACC.vwGLVoucherItem g
//    ON g1.GLVoucherId = g.GLVoucherRef) GLVoucher
//  ON GLVoucher.VoucherRef = vi.VoucherRef
//WHERE 
// (VoucherDate >= @FromDate )
//AND (VoucherDate <= @ToDate)
//AND ((VoucherType IN (1, 2, 6)) OR (VoucherType = N'5' AND VoucherDate <= @LastDayOfFirstYear) OR (VoucherType = N'-1' AND VoucherDate >= @FirstDayOfLastYear) OR (VoucherType = N'-1'))
//AND DLRef=(SELECT csT_dl FROM dbo.PRG_Customers WHERE cst_id = " + UserId + @")
//--AND DLRef=399";
                        SqlParameter[] parameters2 = new SqlParameter[]
                        {
                            new SqlParameter("@type", false),
                            new SqlParameter("@Dlref", dlref.Rows[0][0]),
                            new SqlParameter("@FromDate", DTNOW),
                            new SqlParameter("@ToDate", DTNOW2),
                            new SqlParameter("@LastDayOfFirstYear", DTNOW),
                            new SqlParameter("@FirstDayOfLastYear", DTNOW2),
                        };
                        DataTable dt_ditales = cp.select_row("SpSelectMororHesabfooter", parameters2);
                        dt_ditales.Columns.Add("moghayese", typeof(decimal));
                        //dt_ditales.Columns["moghayese"].Caption = "مانده در خط";
                        DataView dv = new DataView();
                        dv = dt_ditales.DefaultView;
                        dv.ApplyDefaultSort = false;
                        if (checkmororhesab == 0)
                        {
                            dv.Sort = "VoucherDate,RowNumber";
                        }
                        if (checkmororhesab == 1)
                        {
                            dv.Sort = "HeaderDate";
                        }
                        dt_ditales = dv.ToTable();
                        decimal before = 0;
                        string dltitle = "";
                        foreach (DataRow dr in dt_ditales.Rows)
                        {
                            if (checkmororhesab == 0)
                            {
                                dr["moghayese"] = before + (decimal.Parse(dr["Debit"].ToString()) - decimal.Parse(dr["Credit"].ToString()));
                            }
                            if (checkmororhesab == 1)
                            {
                                dr["moghayese"] = before + decimal.Parse(dr["AmountInBaseCurrency"].ToString());
                            }
                            dr.AcceptChanges();
                            before = decimal.Parse(dr["moghayese"].ToString());
                        }
                        Pages = 1;// dt_ditales.Rows.Count / 15;
                        //if (Pages < 1)
                        //    Pages = 1;
                        //if (dt_ditales.Rows.Count > 15)
                        //{
                        //    dt_ditales = dt_ditales.AsEnumerable().Skip((request.page_number - 1) * 15)
                        //        .CopyToDataTable();
                        //    dt_ditales = dt_ditales.AsEnumerable().Take(15).CopyToDataTable();
                        //}


                        foreach (DataRow row in dt_ditales.Rows)
                        {
                            turnover.Add(new crmBillResponseModel.turnover()
                            {
                                Row = row["RowNumber"].ToInt(),
                                Date = row["VoucherDate"].ToString(),
                                Description = row["Description"].ToString(),
                                Debit = row["Debit"].ToDecimal(),
                                Credit = row["Credit"].ToDecimal(),
                                Remaining = row["moghayese"].ToDecimal()
                            });
                        }

                        header.Turnovers = new ListResponse()
                        {
                            Pages = Pages,
                            Page_row = request.page_number,
                            List = turnover
                        };
                    }
                    else
                    {
                        //header.Turnovers = null;
                    }
                    response_bill = new crmBillResponseModelView(header);
                }
                else
                {
                    response_bill = new crmBillResponseModelView(ErrorCode.NotFound.GetHashCode(), "رکوردی یافت نشد");
                }
            }
            catch (Exception e)
            {
                if (cls_public.sqlError.success == false)
                    response_bill = new crmBillResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
                else
                    response_bill = new crmBillResponseModelView(ErrorCode.DBO_Error.GetHashCode(), e.Message);
            }
            return JsonConvert.SerializeObject(response_bill);
        }

        [HttpPost]
        [Route("api/[Controller]/DeleteRequest/{Formid}")]
        [Authorize]
        public string DeleteRequest(int Formid)
        {
            bool check = false;
            string str = "";
            int insertbuyPortal = clsProperties.GetPropertise("insertbuyPortal").ToInt();
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();

            if (insertbuyPortal == 0)
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@rb", Formid),
                };
                if (cp.delete("SpdeletePRG_RequestBuy", parameters))
                {
                    check = false;
                    str = "حذف درخواست با موفقیت انجام شد";
                    return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "حذف درخواست با موفقیت انجام شد");

                }
                else
                {
                    str = "حذف درخواست با خطا مواجه شد";

                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), str);

                }
            }
            if (insertbuyPortal == 1)
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@qu", Formid),
                };
                if (cp.delete("SpdeletePRG_Quotation", parameters))
                {
                    check = false;
                    str = "حذف پیش فاکتور با موفقیت انجام شد";


                }
                else
                {
                    str = "حذف پیش فاکتور با خطا مواجه شد";
                    return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "حذف پیش فاکتور با خطا مواجه شد");

                }
            }
            if (check)
            {
                return messagebase.messages(true, ErrorCode.Created.GetHashCode(), str);
            }
            else
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), str);
            }
        }
    }
}
