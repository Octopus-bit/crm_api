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

namespace sigma_api.Controllers
{

    public class androidController : Controller
    {
        // کنترلر های اپلیکیشن اندروید
        cls_public cp = new cls_public();
        androidLoginResponseModelView response_login;
        androidCallResponseModelView response_call;
        androidCallBatchResponseModelView response_callBatch;

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
                new SqlParameter("password",password),
                new SqlParameter("Type","0") // ( نوع یوزر 2 دسته دارد ( 0 = کاربر سیستمی / 1 = مشتری های سیستم
            };
            DataTable dt = cp.select_row("API_CheckUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                response_login = new androidLoginResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {

                if (dt.Rows.Count == 1)
                {
                    int user_Id = dt.Rows[0]["USR_id"].ToInt(); // ایدی کاربر
                    string user_Name = dt.Rows[0]["Name"].ToString(); // نام کاربر
                    bool Status = dt.Rows[0]["USR_type"].ToBoolean(); // وضعیت کاربر فعال یا غیر فعال
                    bool UserMobile = dt.Rows[0]["USR_mobile"].ToBoolean(); // ایا کاربر دسترسی به اپ موبایل دارد ؟
                    bool SoundRecord = dt.Rows[0]["USR_SoundRecord"].ToBoolean(); // دسترسی برای ثبت توضیحات پس از تماس
                    bool GpsLocation = dt.Rows[0]["USR_GpsLocation"].ToBoolean(); // ثبت لوکیشن بر قراری تماس
                    if (Status == false)
                    {
                        response_login = new androidLoginResponseModelView(ErrorCode.NoAccess.GetHashCode(), "حساب کاربری غیرفعال میباشد");
                    }
                    else if (UserMobile == false)
                    {
                        response_login =
                            new androidLoginResponseModelView(ErrorCode.SpecialAccess.GetHashCode(), "این حساب کاربری به اپلیکیشن موبایل دسترسی ندارد");
                    }
                    else
                    {
                        string token = TokenManagement.WriteToken(user_Id, TypeToken.user.GetHashCode());
                        if (TokenManagement.IsCreated)
                        {
                            androidLoginResponseModel data = new androidLoginResponseModel();
                            data.id = token;
                            data.fullname = user_Name;
                            data.access = new androidLoginResponseModel.data();
                            data.access.SoundRecord = SoundRecord.ToBoolean();
                            data.access.GpsLocation = GpsLocation.ToBoolean();
                            response_login = new androidLoginResponseModelView(data);
                            HttpContext.Response.Headers.Add("Token", token);
                        }
                        else
                        {
                            response_login =
                                new androidLoginResponseModelView(ErrorCode.Authentication.GetHashCode(),
                                    "احراز هویت با مشکل مواجه شد");
                        }
                    }
                }
                else
                {
                    response_login = new androidLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }

            return JsonConvert.SerializeObject(response_login);
        }


        [HttpPost]
        [Route("api/[Controller]/call")]
        [Authorize]
        public string call(androidCallRequestModelView request)
        {
            string CustomerFullName = "";
            string CRC_createdate = request.date;
            string CRC_TimeCall = request.time;
            string mobile = request.mobile;
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

            if (mobile.StartsWith("98") || mobile.StartsWith("+98") || mobile.StartsWith("09"))
            {
                if (mobile.StartsWith("98"))
                {
                    mobile = "+" + mobile;
                    mobile = mobile.Replace("+98", "0");
                }

                if (mobile.StartsWith("+98"))
                {
                    mobile = mobile.Replace("+98", "0");
                }
            }
            else
            {
                foreach (string code in new clsProperties().provincePhoneCodes)
                {
                    if (mobile.StartsWith(code))
                    {
                        mobile.Replace(code, "");
                        break;
                    }
                }
            }

            SqlParameter[] parameters =
            {
                    new SqlParameter("USR_id", UserId),
            };
            
            DataTable dt = cp.select_row("SPandroid_GetAccessUser", parameters);
            SqlParameter[] parameters2 =
            {
                    new SqlParameter("Mobile", mobile),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByMobile", parameters2);
            if (dt_InfoCustomer.Rows.Count > 0)
            {
                CustomerFullName = dt_InfoCustomer.Rows[0]["fullname"].ToString();
            }
            SqlParameter[] parameters3 =
            {
                    new SqlParameter("CRC_createdate", CRC_createdate),
                    new SqlParameter("CRC_TimeCall", CRC_TimeCall),
                    new SqlParameter("mobile", mobile),
                    new SqlParameter("State", State),
                    new SqlParameter("Type", Type),
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("Duration", Duration),
                };
            string id = cp.insert_getid("SPandroid_CreateCall_insert", parameters3);
            if (cls_public.sqlError.success == false)
            {
                response_call = new androidCallResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(),
                    cls_public.sqlError.message);
            }
            else
            {
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

                    if(item.mobile.StartsWith("98") || item.mobile.StartsWith("+98") || item.mobile.StartsWith("09"))
                    {
                        if (item.mobile.StartsWith("98"))
                        {
                            item.mobile = "+" + item.mobile;
                            item.mobile = item.mobile.Replace("+98", "0");
                        }

                        if (item.mobile.StartsWith("+98"))
                        {
                            item.mobile = item.mobile.Replace("+98", "0");
                        }
                    }
                    else
                    {
                        foreach (string code in new clsProperties().provincePhoneCodes)
                        {
                            if (item.mobile.StartsWith(code))
                            {
                                item.mobile.Replace(code, "");
                                break;
                            }
                        }
                    }

                    SqlParameter[] parameters2 =
                    {
                    new SqlParameter("Mobile", item.mobile),
                };
                    DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByMobile", parameters2, transaction);
                    if (dt_InfoCustomer.Rows.Count > 0)
                    {
                        CustomerFullName = dt_InfoCustomer.Rows[0]["fullname"].ToString();
                    }
                    SqlParameter[] parameters3 =
                    {
                    new SqlParameter("CRC_createdate", item.date),
                    new SqlParameter("CRC_TimeCall", item.time),
                    new SqlParameter("mobile", item.mobile),
                    new SqlParameter("State", item.state),
                    new SqlParameter("Type", item.type),
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("Duration", item.duration),
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
                            SqlParameter[] parameters =
                            {
                            new SqlParameter("CRC_id", id),
                            new SqlParameter("cammands", item.Description.commands),
                            new SqlParameter("alarm_date", item.Description.alarm_date),
                            new SqlParameter("alarm_time", item.Description.alarm_time),
                            new SqlParameter("alarm_desc", item.Description.alarm_desc),
                            new SqlParameter("location", item.Description.location)
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
            SqlParameter[] parameters =
            {
                new SqlParameter("CRC_id",call_id),
                new SqlParameter("cammands",cammands.TryString()),
                new SqlParameter("alarm_date",alarm_date),
                new SqlParameter("alarm_time",alarm_time),
                new SqlParameter("alarm_desc",alarm_desc.TryString()),
                new SqlParameter("location",location.TryString())
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
                new SqlParameter("location",location.TryString())
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

    }
}
