using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sigma_api;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Net.Http;
namespace sigma_api.Classes
{
    public class cls_TblSendPdf : Connection.sql_db
    {
        cls_public cp = new cls_public();
        public string TblSendPdf_insert(string url)
        {
            try
            {
                string id = null;
                string[] path = url.Split(@"\");
                if (path[path.Length - 1].EndsWith(".pdf"))
                {
                    string[] pdfname = path[path.Length - 1].Replace(".pdf", null).Split("-");
                    id = pdfname[1] + "-" + pdfname[2];
                }
                SqlParameter[] parameters =
                {
                    new SqlParameter("id",id),
                    new SqlParameter("local_url",url)
                };
                cp.insert("PRG_TblSendPdf_insert", parameters);
                return id;/*Security.coding.EncryptString(scaler.ToString());*/
            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
        }

        public DataTable TblSendPdf_SelectLocalUrlByid(string id)
        {
            SqlParameter[] parameters =
                {
                    new SqlParameter("id",id),
                };
            DataTable dt = new DataTable();
            dt = cp.select_row("PRG_TblSendPdf_SelectRow", parameters);
            if (dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }
    }
}
