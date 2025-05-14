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
    public class CrmController : Controller
    {
        cls_public cp = new cls_public();

        [HttpPost]
        [Route("api/[controller]/login")]
        public string login(crmLoginRequestModelView request)
        {
            crmLoginResponseModelView response_login;
            string username, password;
            username = request.username;
            password = request.password;//coding.SigmaEncryptString(coding.SafeFarsiStr(request.password));
            SqlParameter[] parameters =
            {
                new SqlParameter("username",username),
                new SqlParameter("password",password),
            };
            DataTable dt = cp.select_row("API_CheckCustomer", parameters);
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
                        crmLoginResponseModel data = new crmLoginResponseModel();
                        data.id = ID;
                        data.Type = Type;
                        data.FirstName = FirstName;
                        data.LastName = LastName;
                        data.MeliCode = MeliCode;
                        data.Email = Email;
                        data.Birthdate = Birthdate;
                        response_login = new crmLoginResponseModelView(data);
                    }
                }
                else
                {
                    response_login = new crmLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }

            return JsonConvert.SerializeObject(response_login);
        }
    }
}
