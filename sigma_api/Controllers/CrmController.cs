using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.ModelView;
using sigma_api.Models;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Security;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace sigma_api.Controllers
{
    public class CrmController : Controller
    {
        cls_public cp = new cls_public();
        crmLoginResponseModelView response_login;
        crmRegistryCodeResponseModelView response_RegistryCode;

        [HttpPost]
        [Route("api/[controller]/login")]
        public string login(crmLoginRequestModelView request)
        {
            string username, password;
            username = request.username;
            password = coding.SigmaEncryptString(/*coding.SafeFarsiStr(*/request.password/*)*/);
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
        public string signup()
        {
            return "";
        }

        [HttpPost]
        [Route("api/[controller]/forgot-password")]
        public string RegistryCode(crmRegistryCodeRequestModelView request)
        {
            string mobile;
            string code;
            mobile = request.mobile;
            try
            {
                code = request.code.ToString();
            }
            catch
            {
                code = null;
            }


            SqlParameter[] parameters2 =
            {
                new SqlParameter("Mobile", mobile),
                new SqlParameter("RegistryCode", string.IsNullOrEmpty(code)?DBNull.Value:code),
            };
            int res_code = cp.insert_getid("API_RegistryCodeUsers", parameters2).ToInt();
            if (res_code == 1)
            {
                response_RegistryCode = new crmRegistryCodeResponseModelView(true,ErrorCode.Send.GetHashCode(), "code generated");
            }
            else
            {
                if (res_code == 0)
                {
                    response_RegistryCode = new crmRegistryCodeResponseModelView(false,ErrorCode.Conflict.GetHashCode(),
                        "تایید حساب کاربری با مشکل مواجه شده است");
                }
                else if (res_code > 1)
                {

                    string token = TokenManagement.WriteToken(res_code, TypeToken.customer.GetHashCode());
                    if (TokenManagement.IsCreated)
                    {
                        crmLoginResponseModel data = new crmLoginResponseModel();
                        data.id = token;
                        response_login = new crmLoginResponseModelView(data);
                        response_RegistryCode = new crmRegistryCodeResponseModelView(true,ErrorCode.Created.GetHashCode(), "registered", data);
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
        [HttpPut]
        [Route("api/[controller]/reset-password")]
        public string CheangPassword([Required] string NewPassword)
        {
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {

                m.code = ErrorCode.Authentication.GetHashCode();
                m.success = false;
                m.message = "عدم احراز هویت";
                
                return JsonConvert.SerializeObject(m);
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("UserId", UserId),
                new SqlParameter("Password", coding.SigmaEncryptString(NewPassword)),
            };
            if (cp.update("API_CheangPasswordUser",parameters))
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
    }
}
