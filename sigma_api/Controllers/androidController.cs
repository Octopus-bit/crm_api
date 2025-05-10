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
namespace sigma_api.Controllers
{
    public class androidController : Controller
    {
        cls_public cp = new cls_public();
        androidLoginResponseModelView response_login;
        androidCallResponseModelView response_call;
        [HttpPost]
        [Route("api/[Controller]/login")]
        public string login(androidLoginRequestModelView request)
        {
            string username, password;
            username = request.username;
            password = coding.SigmaEncryptString(coding.SafeFarsiStr(request.password));
            SqlParameter[] parameters =
            {
                new SqlParameter("username",username),
                new SqlParameter("password",password),
            };
            DataTable dt = cp.select_row("SPandroid_CheckUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                response_login = new androidLoginResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {

                if (dt.Rows.Count == 1)
                {
                    int user_Id = dt.Rows[0]["USR_id"].ToInt();
                    string user_Name = dt.Rows[0]["Name"].ToString();
                    bool Status = dt.Rows[0]["USR_type"].ToBoolean();
                    bool UserMobile = dt.Rows[0]["USR_mobile"].ToBoolean();
                    bool SoundRecord = dt.Rows[0]["USR_SoundRecord"].ToBoolean();
                    bool GpsLocation = dt.Rows[0]["USR_GpsLocation"].ToBoolean();
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
                        androidLoginResponseModel data = new androidLoginResponseModel();
                        data.id = user_Id;
                        data.fullname = user_Name;
                        data.access = new androidLoginResponseModel.data();
                        data.access.SoundRecord = SoundRecord.ToBoolean();
                        data.access.GpsLocation = GpsLocation.ToBoolean();
                        response_login = new androidLoginResponseModelView(data);
                    }
                }
                else
                {
                    response_login = new androidLoginResponseModelView(ErrorCode.Authentication.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }

            return JsonConvert.SerializeObject(response_login);
        }

        [HttpPost]
        [Route("api/[Controller]/call")]
        public string call(androidCallRequestModelView request)
        {
            string CRC_createdate = request.date;
            string CRC_TimeCall = request.time;
            string mobile = request.mobile;
            int State = request.state;
            int user_id = request.user_id;
            string Duration = request.duration;
            string Guid = request.Guid;
            if (mobile.StartsWith("98"))
            {
                mobile = "+" + mobile;
                mobile = mobile.Replace("+98", "0");
            }
            if (mobile.StartsWith("+98"))
            {
                mobile = mobile.Replace("+98", "0");
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("USR_id",user_id),
            };
            DataTable dt = cp.select_row("SPandroid_GetAccessUser", parameters);
            SqlParameter[] parameters2 =
            {
                new SqlParameter("CRC_createdate",CRC_createdate),
                new SqlParameter("CRC_TimeCall",CRC_TimeCall),
                new SqlParameter("mobile",mobile),
                new SqlParameter("State",State),
                new SqlParameter("user_id",user_id),
                new SqlParameter("Duration",Duration),
                new SqlParameter("Guid",Guid)
            };
            string id = cp.insert_getid("SPandroid_CreateCall_insert", parameters2);
            if (cls_public.sqlError.success == false)
            {
                response_call = new androidCallResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {
                if (id !="0" && dt.Rows.Count >= 1)
                {
                    
                    string[] str = id.Split('?');

                    androidCallResponseModel data = new androidCallResponseModel();
                    data.id = str[0].ToInt();
                    data.Guid = Guid;
                    if (str.Length > 1)
                        data.CustomersName = str[1].ToString();
                    else
                    {
                        data.CustomersName = "";
                    }
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
                        response_call = new androidCallResponseModelView(ErrorCode.BadRequest.GetHashCode(), $"{cls_public.sqlError.message}");
                    else if (cls_public.sqlError.message == "DuplicateGuid")
                        response_call = new androidCallResponseModelView(ErrorCode.Duplicate.GetHashCode(), $"{cls_public.sqlError.message}");
                    else
                        response_call = new androidCallResponseModelView(ErrorCode.DBO_Error.GetHashCode(), $"{cls_public.sqlError.message}");

                }
            }

            return JsonConvert.SerializeObject(response_call);
        }

        [HttpPost]
        [Route("api/[Controller]/call/{call_id}")]
        public string call_update(androidCallRequestModelView.androidUpdateCallRequestModelView request)
        {
            //int call_id = Request.RouteValues["call_id"].ToInt();
            string call_id = Request.RouteValues["call_id"].ToString();
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

            messagebase mes = new messagebase();
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
        public string call_update_location(androidCallRequestModelView.androidUpdateCallLocationRequestModelView request)
        {
            //int call_id = Request.RouteValues["call_id"].ToInt();
            string call_id = Request.RouteValues["call_id"].ToString();
            string location = request.location;
            SqlParameter[] parameters =
            {
                new SqlParameter("CRC_id",call_id),
                new SqlParameter("location",location.TryString())
            };
            messagebase mes = new messagebase();
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
