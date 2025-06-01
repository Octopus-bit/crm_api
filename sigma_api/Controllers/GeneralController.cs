using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Models;
using sigma_api.ModelView;
using sigma_api.Security;

namespace sigma_api.Controllers
{
    public class GeneralController : Controller
    {
        private cls_public cp = new cls_public();
        private generalInfoCustomerResponseModelView response_InfoCustomer;
        private generalSystemSettingsResponseModelView response_SystemSettings;

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/get-info-customer/{mobile?}")]
        public string getInfoCustomer([Required] string mobile)
        {
            messagebase mes = new messagebase();
            if (string.IsNullOrEmpty(mobile))
            {
                mes.success = false;
                mes.code = ErrorCode.BadRequest.GetHashCode();
                mes.message = "not found mobile";
                return JsonConvert.SerializeObject(mes);
            }
            if(mobile.StartsWith("98") || mobile.StartsWith("+98") || mobile.StartsWith("09"))
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
            SqlParameter[] parameters2 =
            {
                new SqlParameter("Mobile", mobile),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoByMobile", parameters2);
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
                response_InfoCustomer = new generalInfoCustomerResponseModelView(data);
            }
            else
            {
                response_InfoCustomer = new generalInfoCustomerResponseModelView(null);
            }
            return JsonConvert.SerializeObject(response_InfoCustomer);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/System-settings")]
        public string System()
        {
            try
            {

                generalSystemSettingsResponseModel system = new generalSystemSettingsResponseModel()
                {
                    Company = clsProperties.GetPropertise("txtnameforoshgah").ToString(),
                    Logo = clsProperties.GetPropertise("logo").ToString(),
                    Version = typeof(Program).Assembly.GetName().Version?.ToString(),
                    Column_Unit2 = clsProperties.GetPropertise("SubunitQuotation").ToBoolean(),
                    ExpirationQuotations = clsProperties.GetPropertise("expireddayQuotation").ToInt(),
                };
                response_SystemSettings = new generalSystemSettingsResponseModelView(system);

            }
            catch (Exception e)
            {
                response_SystemSettings =
                    new generalSystemSettingsResponseModelView(ErrorCode.Conflict.GetHashCode(), e.Message);
            }

            return JsonConvert.SerializeObject(response_SystemSettings);
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/IsAuthorized")]
        public string IsAuthorized()
        {
            return messagebase.messages(true, ErrorCode.Send.GetHashCode(), "دسترسی مجاز");
        }

        [Authorize]
        [HttpDelete]
        [Route("api/[controller]/logout")]
        public string logout()
        {
            string token = HttpContext.Items["token"].ToString();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (cp.command($"UPDATE dbo.API_Tokens SET tk_AtCreate = GETDATE()-1 WHERE tk_Token = '{token}' and tk_UserIdRef = {UserId}"))
            {
                return messagebase.messages(true, ErrorCode.Created.GetHashCode(), "Token successfully revoked.");
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Conflict.GetHashCode(), "Token was not successfully revoked.");
            }
        }
    }
}
