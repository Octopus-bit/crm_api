using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using sigma_api.Classes;
using System.Security;
using System.Net;
using System.Net.Http;
using System.IO;
using sigma_api;
using sigma_api.ModelView;
using sigma_api.Models;
using Newtonsoft.Json;
using System.Text;

namespace sigma_api.Controllers
{
    public class pdfController : Controller
    {
        cls_TblSendPdf db = new cls_TblSendPdf();

        [HttpPost]
        [Route("api/[Controller]/add")]
        public string add_pdf(pdfRequestModelView pdf)
        {
            pdfResponseModelView res;
            pdfModel pdf_web_url = new pdfModel();
            string id = db.TblSendPdf_insert(pdf.localurl);
            if (!id.StartsWith("Error") && id != null)
            {
                id = id;//WebUtility.UrlEncode(id);
                if (Request.IsHttps)
                    pdf_web_url.weburl = "https://" + Request.Host.ToString() + "/api/pdf/info/" + id;
                else
                    pdf_web_url.weburl = "http://" + Request.Host.ToString() + "/api/pdf/info/" + id;
                res = new pdfResponseModelView(pdf_web_url);
            }
            else
                res = new pdfResponseModelView(id);
            string json = JsonConvert.SerializeObject(res);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");
            return json;
        }

        [HttpGet]
        [Route("api/[Controller]/info/{id}")]
       /* public IActionResult open()
        {
            int id = int.Parse(Request.RouteValues["id"].ToString());//int.Parse(Security.coding.DecryptString(Request.RouteValues["id"].ToString()));
            string url = db.Get_Url(id).Rows[0]["urls"].ToString();
            return Redirect(url);

        }

        [HttpGet]
        [Route("api/[Controller]/open")]*/
        public IActionResult open()

        {

            // آدرس فایل PDF محلی را مشخص کنید
            string id = Request.RouteValues["id"].ToString();//int.Parse(Security.coding.DecryptString(Request.RouteValues["id"].ToString()));
            DataTable dt = db.TblSendPdf_SelectLocalUrlByid(id);
            string url = dt.Rows[0]["local_url"].ToString();
            var pdfPath = Path.Combine(Directory.GetCurrentDirectory(), url);


            if (!System.IO.File.Exists(pdfPath))

            {

                return NotFound(); // اگر فایل موجود نباشد

            }


            var fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read);

            return File(fileStream, "application/pdf", Path.GetFileName(pdfPath));

        }

    }
}
