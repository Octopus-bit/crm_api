using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/get-info-customer/{mobile}")]
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
            SqlParameter[] parameters2 =
            {
                new SqlParameter("Mobile", mobile),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_CustomerInfo", parameters2);
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
                data.Status = dt_InfoCustomer.Rows[0]["Status"].ToInt();
                response_InfoCustomer = new generalInfoCustomerResponseModelView(data);
            }
            else
            {
                response_InfoCustomer = new generalInfoCustomerResponseModelView(null);
            }
            return JsonConvert.SerializeObject(response_InfoCustomer);
        }

        [Authorize]
        [HttpDelete]
        [Route("api/[controller]/logout")]
        public string logout()
        {
            string token = coding.SigmaEncryptString(HttpContext.Items["token"].ToString());
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (cp.command($"UPDATE dbo.API_Tokens SET tk_AtCreate = GETDATE()-1 WHERE tk_Token = '{token}' and tk_UserIdRef = {UserId}"))
            {
                return messagebase.messages(true, ErrorCode.Send.GetHashCode(), "Token successfully revoked.");
            }
            else
            {
                return messagebase.messages(false, ErrorCode.Created.GetHashCode(), "Token was not successfully revoked.");
            }
        }
    }
}
