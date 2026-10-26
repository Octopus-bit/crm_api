using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using FastReport.Export.PdfSimple;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using sigma_api.Controllers;
using sigma_api.Models;
using sigma_api.ModelView;
using static iText.IO.Image.Jpeg2000ImageData;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using FastReport;
using FastReport.Export.PdfSimple;

namespace sigma_api.Classes
{
    public class cls_public : Connection.sql_db
    {
        SqlConnection _con;
        SqlDataAdapter _dataAdapter;
        SqlCommand _command;
        private const int CommandTimeoutSeconds = 180;
        public static messagebase sqlError = new messagebase();
        public cls_public()
        {
            sqlError.message = null;
            try
            {
                _con = connct;//new SqlConnection("Data Source=192.168.233.10; Initial Catalog=SigmaCrm; User ID=sa; Password=sa@12345678;");//connct;
                _con.Open();
                _con.Close();
                sqlError.success = true;
            }
            catch (Exception e)
            {
                sqlError.success = false;
                sqlError.message = "پایگاه داده در دسترس نیست";
            }
        }

        public SqlConnection connect()
        {
            return _con;
        }

        public void close()
        {
            if (_con.State == ConnectionState.Open)
                _con.Close();
        }

        public SqlTransaction Transaction(string name) // ایجاد ترنزکشن
        {
            if (_con.State != ConnectionState.Open)
                _con.Open();
            return _con.BeginTransaction(name);
        }

        private void CloseConnectionIfIdle(SqlTransaction transaction = null)
        {
            if (transaction == null && _con.State == ConnectionState.Open)
                _con.Close();
        }
        //public void DaynamicReportDatabase(DataRow currentRow, DataTable DtprintDesign, DataTable DtDtsign = null)
        //{
        //    try
        //    {
        //        byte[] sendfile;
        //        byte[] reportBytes;
        //        reportBytes = (byte[])currentRow["DR_ReportData"];// (byte[])cmd.ExecuteScalar();
        //        if (reportBytes == null || reportBytes.Length == 0)
        //        {
        //            //MessageBox.Show("فایل گزارش در دیتابیس یافت نشد یا خالی است.");
        //            return;
        //        }

        //        Report report = new Report();
        //        try
        //        {
        //            using (MemoryStream ms = new MemoryStream(reportBytes))
        //            {
        //                ms.Position = 0;
        //                report.Load(ms);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(
        //                ex.ToString());
        //        }
        //        // بارگذاری گزارش از حافظه
        //        using (MemoryStream ms = new MemoryStream(reportBytes))
        //        {
        //            ms.Position = 0;
        //            report.Load(ms);
        //        }

        //        // ثبت منبع داده
        //        report.RegisterData(DtprintDesign, "Report");
        //        report.GetDataSource("Report").Enabled = true;
        //        if (DtDtsign != null)
        //        {
        //            report.RegisterData(DtDtsign, "Signs");
        //            report.GetDataSource("Signs").Enabled = true;
        //        }
        //        // تنظیم پارامترها (در صورت نیاز)
        //        report.SetParameterValue("Filter", "");
        //        report.Prepare();
        //        string ExePath = Assembly.GetEntryAssembly()?.Location;
        //        string directoryPath = AppContext.BaseDirectory;
        //        if (!Directory.Exists(directoryPath + "PrintPdf"))
        //        {
        //            Directory.CreateDirectory(directoryPath + "PrintPdf");
        //        }
        //        Random rand = new Random();
        //        string crand = rand.Next(1111, 9999).ToString();
        //        string downloadsPath = directoryPath + "PrintPdf\\" + "PrintPdf" + "_" + crand + ".pdf";
        //        string filename = "PrintPdf" + "_" + crand;
        //        report.Export(new PDFSimpleExport(), downloadsPath);
        //        try
        //        {
        //            sendfile = CrmController.ReadFile(downloadsPath);
        //        }
        //        catch
        //        {
        //            sendfile = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ErrorStr = ex.Message;
        //    }

        //}

        public static string convertTextToMoneyStr(string StrPrice)
        {
            string resualt = "";
            decimal Number;

            if (decimal.TryParse(StrPrice, out Number))
            {
                resualt = string.Format("{0:N0}", Number);
            }
            return resualt;
        }

        public DateTime GetDateTimeInvariantCulture(SqlTransaction transaction = null)
        {
            DateTime getdate = new DateTime();
            SqlCommand cmdLog = new SqlCommand("", _con);
            cmdLog.Transaction = transaction;
            cmdLog.CommandType = CommandType.Text;
            cmdLog.CommandTimeout = CommandTimeoutSeconds;
            cmdLog.CommandText = "Select getdate()";
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    _con.Open();
                }

                //string getdatee = cmdLog.ExecuteScalar().ToString();
                DateTime dateFromDb = (DateTime)cmdLog.ExecuteScalar();
                string getdatee = dateFromDb.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                getdate = Convert.ToDateTime(getdatee);
                CloseConnectionIfIdle(transaction);
                getdate = Convert.ToDateTime(getdatee);

            }
            catch
            {
                CloseConnectionIfIdle(transaction);

            }
            finally
            {
                CloseConnectionIfIdle(transaction);

            }
            return getdate;
        }
        public DateTime GetDateTime(SqlTransaction transaction = null)
        {
            DateTime getdate = new DateTime();
            SqlCommand cmdLog = new SqlCommand("", _con);
            cmdLog.Transaction = transaction;
            cmdLog.CommandType = CommandType.Text;
            cmdLog.CommandTimeout = CommandTimeoutSeconds;
            cmdLog.CommandText = "Select getdate()";
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    _con.Open();
                }

                string getdatee = cmdLog.ExecuteScalar().ToString();
                CloseConnectionIfIdle(transaction);
                getdate = Convert.ToDateTime(getdatee);

            }
            catch
            {
                CloseConnectionIfIdle(transaction);

            }
            finally
            {
                CloseConnectionIfIdle(transaction);

            }
            return getdate;
        }
        public DataTable select(string StoredProcedure, SqlTransaction transaction = null)
        {
            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                DataTable dt = new DataTable();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                _dataAdapter = new SqlDataAdapter(_command);
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        _con.Open();
                    }
                    _dataAdapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    return null;
                }
            }

        }

        public DataTable select_row(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {

                DataTable dt = new DataTable();
                // _command = new SqlCommand(StoredProcedure, _con);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
               _dataAdapter = new SqlDataAdapter(_command);
                foreach (SqlParameter parameter in parameters)
                {
                    _command.Parameters.Add(parameter);
                }
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    //_command.ExecuteNonQuery();
                    _dataAdapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    Classes.Library.WriteErrorLog(" StoredProcedure :" + StoredProcedure + ex.Message);
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    return null;
                }
            }
        }

        public DataTable select_command(string Select_command, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            using (SqlCommand _command = new SqlCommand(Select_command, _con))
            {
                DataTable dt = new DataTable();
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                _command.Parameters.Clear();
                if (parameters != null)
                    foreach (SqlParameter parameter in parameters)
                    {
                        _command.Parameters.Add(parameter);
                    }
                _dataAdapter = new SqlDataAdapter(_command);
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    _dataAdapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    return null;
                }
            }
        }

        public bool insert(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            var result = 0;
            using (SqlCommand cmd = new SqlCommand(StoredProcedure, _con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = CommandTimeoutSeconds;
                cmd.Transaction = transaction;
                foreach (SqlParameter parameter in parameters)
                {
                    cmd.Parameters.Add(parameter);
                }
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    throw;
                }
            }
            return result > 0;
        }

        public string insert_DatabTable(string StoredProcedure, DataTable Dtinsert, SqlTransaction transaction = null)
        {
            string id = "";
            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                //_command = new SqlCommand(StoredProcedure, _con);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                _command.Parameters.AddWithValue("@TableValidFile", Dtinsert);
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }

                    id = Convert.ToString(_command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    return "0";
                }
            }
            return id;
        }
        //public DataTable executedQuery(string Text)
        //{
        //    DataTable dtresualt = new DataTable();
        //    using (SqlCommand _command = new SqlCommand("Text", _con))
        //    {
        //        Command.CommandType = CommandType.Text;
        //        _DA.SelectCommand = Command;
        //        _DA.Fill(_DS);
        //        dtresualt = _DS.Tables[0];
        //        DataSet _DS = new DataSet();
        //        SqlDataAdapter _DA = new SqlDataAdapter();
        //        SqlCommand Command = new SqlCommand(Text, _con);
        //        try { if (_con.State == System.Data.ConnectionState.Closed) _con.Open(); }
        //        catch (Exception ex) { }
        //        Command.CommandType = CommandType.Text;
        //        _DA.SelectCommand = Command;
        //        _DA.Fill(_DS);
        //        dtresualt = _DS.Tables[0];
                
        //    }
        //    return dtresualt;
        //}
        public string insert_getid(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {

            string id = "";
            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                foreach (SqlParameter parameter in parameters)
                {
                    _command.Parameters.Add(parameter);
                }
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }

                    id = Convert.ToString(_command.ExecuteScalar());
                    CloseConnectionIfIdle(transaction);
                }
                catch (Exception ex)
                {
                    if (transaction == null)
                        _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    return "0";
                }
            }
            return id;
        }
        public DataTable GetSignerForm(FastReport.Report report, int? formtypeid, string formid)
        {
            DataTable DtSigns = new DataTable();
            DataTable dtremindnew = new DataTable();
            using (SqlCommand _command2 = new SqlCommand(@"select USR_sign,dbo.mtsh(IS_SignDate) SignDate,PRG_Users.Name from [dbo].[PRG_InsertSigns]
join PRG_Users on USR_id=IS_usr_id  where  IS_formtypeid=" + formtypeid.ToString() + " and IS_formid=" + formid + @" and [USR_sign] is not null
order by IS_SignDate", _con))
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
        public void SendSmsToDatabase(string type, string formid0, string formid1, string str, string Formid, string MatnStr)
        {
            cls_public cp2 = new cls_public();
            DataTable DtSendSmsClass = new DataTable();
            DtSendSmsClass.Columns.Add("State", typeof(bool));
            DtSendSmsClass.Columns.Add("StateSend", typeof(bool));
            DtSendSmsClass.Columns.Add("ReturnCode", typeof(string));
            DtSendSmsClass.Columns.Add("StringSmsReaualt", typeof(string));

            DataTable DtValidSMS = new DataTable();
            DtValidSMS.Columns.Add("message", typeof(string));
            DtValidSMS.Columns.Add("state", typeof(Int16));
            DtValidSMS.Columns.Add("cst,", typeof(int));
            DtValidSMS.Columns.Add("descnot", typeof(string));
            DtValidSMS.Columns.Add("type", typeof(Int16));
            DtValidSMS.Columns.Add("datee", typeof(DateTime));
            DtValidSMS.Columns.Add("timee", typeof(TimeSpan));
            DtValidSMS.Columns.Add("mobile", typeof(string));
            DtValidSMS.Columns.Add("sendid", typeof(string));
            DtValidSMS.Columns.Add("SMLine", typeof(string));
            DtValidSMS.Columns.Add("Usr", typeof(int));
            DtValidSMS.Columns.Add("Formid", typeof(int));
            DtValidSMS.Columns.Add("LinkPdf", typeof(string));
            DtValidSMS.Columns.Add("BytePdf", typeof(byte[]));
            DataSet _DS = new DataSet();
            SqlDataAdapter _DA = new SqlDataAdapter();
            SqlCommand Command = new SqlCommand("", cp2.connect());
            _DA.SelectCommand = Command;
            Command.CommandType = CommandType.Text;
            Command.CommandText = "select DFS_text,DFS_Post from dbo.PRG_DetailFormatSms where DFS_formid=" + formid0 + " and DFS_formstateid=" + formid1;
            _DA.Fill(_DS);
            DataTable dtSmsFormat = _DS.Tables[0];
            DataTable dtleave = new DataTable();
            string LinkPdf = "";
            if (dtSmsFormat.Rows.Count > 0)
            {
                Nullable<int> str2 = null;
                Nullable<int> usr2 = null;
                DateTime dtsend = cp2.GetDateTime();
                TimeSpan TimeJob = new TimeSpan();
                dtsend = dtsend.AddMinutes(1);
                TimeJob = TimeSpan.Parse(dtsend.ToString("HH:mm"));
                string linenumber = "";

                string bedehi = "";
                string bedehi2 = "";
                if (type == "1")//درخواست خرید
                {
                    _DS = new DataSet();
                    _DA = new SqlDataAdapter();
                    Command = new SqlCommand("", cp2.connect());
                    _DA.SelectCommand = Command;
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = @"SELECT     PRG_RequestBuy.RB_id, dbo.mtsh(CONVERT(date, PRG_RequestBuy.RB_date)) AS datee, PRG_RequestBuy.RB_no, 
                      REPLACE(REPLACE(CAST(ISNULL(PRG_Customers.cst_fname,'') + ' ' + ISNULL(PRG_Customers.cst_laname, '') AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS lname, 
                    (select SUBSTRING( DescriptionList,2,LEN(DescriptionList)-1) from ( SELECT
     rbip.RBI_RB_id tbiid,
     STUFF(
     (SELECT  DISTINCT ' , ' +REPLACE(REPLACE(CAST(stc.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705))   
FROM       PRG_StockSep stc inner join PRG_RequestBuyItem rbi 
on stc.StockID=rbi.RBI_stock
where rbi.RBI_RB_id=rbip.RBI_RB_id
          FOR XML PATH (''))
          , 1, 1, '')  AS DescriptionList
FROM PRG_StockSep AS p
inner join PRG_RequestBuyItem rbip 
on p.StockID=rbip.RBI_stock
GROUP BY rbip.RBI_RB_id) F
where F.tbiid=PRG_RequestBuy.RB_id) stocktitle, dbo.ArabicPersion(PRG_Customers_1.cst_fname + ' ' + ISNULL(PRG_Customers_1.cst_laname, '') ) AS tamin, 
                     dbo.ArabicPersion( PRG_Customers_2.cst_fname + ' ' + ISNULL(PRG_Customers_2.cst_laname, '')) AS Vaset, CASE PRG_RequestBuy.RB_typebuy WHEN 0 THEN REPLACE(REPLACE(CAST('نقدي' AS nvarchar(MAX)), 
                      NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) WHEN 1 THEN REPLACE(REPLACE(CAST('اعتباري' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) 
                      WHEN 2 THEN REPLACE(REPLACE(CAST('نقدي - اعتباري' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) END AS typebuy, PRG_Loockup_1.LKP_name, 
                     (select sum(RBI_metraj) from dbo.PRG_RequestBuyItem where RBI_RB_id=PRG_RequestBuy.RB_id) as RBIvalue, 
                     (select sum(RBI_carton) from dbo.PRG_RequestBuyItem where RBI_RB_id=PRG_RequestBuy.RB_id) as RBIvalueSecondary, 
PRG_Loockup.LKP_name AS typecar, PRG_Users_1.Name, PRG_Users.Name AS kasrshenas, PRG_Users.Phone SalesExpertMobile, PRG_RequestBuy.RB_cst_id, 
                      CASE RB_type WHEN 0 THEN 'ثبت شده' WHEN 1 THEN 'تاييد شده' WHEN 3 THEN 'ابطال شده' END AS RB_type, PRG_Users_2.Name AS confirmer, dbo.mtsh(CONVERT(date, PRG_RequestBuy.RB_confirmdate)) AS RB_confirmdate, 
                      PRG_RequestBuy.RB_type AS rbtype, CASE changestatePrs WHEN 1 THEN 'بله' WHEN 0 THEN 'خير' END AS RB_changestatePrs, PRG_Customers.csT_dl,
                          (SELECT     QU_type
                            FROM          PRG_Quotation
                            WHERE      (QU_rb_id = PRG_RequestBuy.RB_id)) AS Quotation, PRG_RequestBuy.RB_Tamin, PRG_RequestBuy.RB_Vaset
,PRG_RequestBuy.RB_SalesExpert,PRG_RequestBuy.RB_creator,dbo.ArabicPersion(locostan.title ) locostan,dbo.ArabicPersion(loccity.title) loccity,dbo.ArabicPersion(locCountry.title) locCountry,
(select sum(RBI_metraj) from dbo.PRG_RequestBuyItem where RBI_RB_id=PRG_RequestBuy.RB_id) RBImetraj,
(select sum(RBI_carton) from dbo.PRG_RequestBuyItem where RBI_RB_id=PRG_RequestBuy.RB_id) RBIcarton,    
 (select sum(RBI_palet) from dbo.PRG_RequestBuyItem where RBI_RB_id=PRG_RequestBuy.RB_id) RBIpalet FROM  
PRG_RequestBuy INNER JOIN
                      PRG_Customers ON PRG_RequestBuy.RB_cst_id = PRG_Customers.cst_id INNER JOIN
                      PRG_Users AS PRG_Users_1 ON PRG_RequestBuy.RB_creator = PRG_Users_1.USR_id LEFT OUTER JOIN
                      PRG_Loockup ON PRG_RequestBuy.RB_typecar = PRG_Loockup.LKP_id LEFT OUTER JOIN
                      PRG_Users ON PRG_RequestBuy.RB_SalesExpert = PRG_Users.USR_id LEFT OUTER JOIN
                      PRG_Customers AS PRG_Customers_1 ON PRG_RequestBuy.RB_Tamin = PRG_Customers_1.cst_id LEFT OUTER JOIN
                      PRG_Customers AS PRG_Customers_2 ON PRG_RequestBuy.RB_Vaset = PRG_Customers_2.cst_id LEFT OUTER JOIN
                      PRG_Loockup AS PRG_Loockup_1 ON PRG_RequestBuy.RB_Creditperiod = PRG_Loockup_1.LKP_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_2 ON PRG_RequestBuy.RB_confirmer = PRG_Users_2.USR_id inner join PRG_Location loccity on loccity.locationid=PRG_Customers.cst_city
                      inner join  PRG_Location locostan on locostan.locationid=PRG_Customers.cst_ostan 
 LEFT join PRG_Location locCountry on locCountry.locationid=PRG_Customers.cst_Country
                      where PRG_RequestBuy.RB_id = " + Formid;
                    _DA.Fill(_DS);
                    dtleave = _DS.Tables[0];
                }
                if (type == "2")//1 پیش فاکتور
                {
                    DataTable dtmali = cp2.select_command("SELECT TNZ_sharh FROM dbo.PRG_Propertise WHERE TNZ_name = 'DataMali'");
                    _DS = new DataSet();
                    _DA = new SqlDataAdapter();
                    Command = new SqlCommand("", cp2.connect());
                    _DA.SelectCommand = Command;
                    Command.CommandType = CommandType.Text;
                    Command.CommandText = @"SELECT     PRG_Quotation.QU_id, CASE WHEN (SELECT COUNT(*) FROM " + dtmali.Rows[0][0].ToString() + @".SLS.Invoice where QuotationRef=PRG_Quotation.QU_QuotationIdSP)=0 THEN 'فاکتور نشده' else 'فاکتور شده' END AS StateQ, dbo.mtsh(CONVERT(date, 
                      PRG_Quotation.QU_date)) AS dateQ, PRG_Quotation.QU_no,PRG_RequestBuy.RB_no, REPLACE(REPLACE(CAST(ISNULL(PRG_Customers.cst_fname,'') + ' ' + ISNULL(PRG_Customers.cst_laname, '') AS nvarchar(MAX)), NCHAR(1610), 
                      NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS customersname, REPLACE(REPLACE(CAST(ISNULL(PRG_Customers_1.cst_fname,'') + ' ' + ISNULL(PRG_Customers_1.cst_laname, '') AS nvarchar(MAX)), NCHAR(1610), 
                      NCHAR(1740)), NCHAR(1603), NCHAR(1705))  AS taminkonandeh, dlsep.Code AS dl, 
                      CASE QU_typesale WHEN 0 THEN 
dbo.ArabicPersion(CAST('نقدي' AS nvarchar(MAX))) 
                      WHEN 1 THEN dbo.ArabicPersion(CAST('اعتباري' AS nvarchar(MAX))) 
                      WHEN 2 THEN dbo.ArabicPersion(CAST('نقدي - اعتباري' AS nvarchar(MAX))) END AS typesale, PRG_Loockup.LKP_name , 
                      PRG_Quotation.QU_NetPrice
,(select sum(QUI_value) from dbo.PRG_QuotationItem where QUI_QU_id=PRG_Quotation.QU_id) as QUIvalue
,(select sum(QUI_valueSecondary) from dbo.PRG_QuotationItem where QUI_QU_id=PRG_Quotation.QU_id) as QUIvalueSecondary
,PRG_Users.Name AS creator, PRG_Users_2.Name AS karshenas, PRG_Users_2.Phone SalesExpertMobile, 
(select SUBSTRING( DescriptionList,2,LEN(DescriptionList)-1) from ( SELECT
     rbip.QUI_QU_id tbiid,
     STUFF(
     (SELECT  distinct ' , ' +REPLACE(REPLACE(CAST(  stc.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705))   
FROM   PRG_StockSep stc inner join PRG_QuotationItem rbi  
on stc.StockID=rbi.QUI_stock
where rbi.QUI_QU_id=rbip.QUI_QU_id
          FOR XML PATH (''))
          , 1, 1, '')  AS DescriptionList
FROM PRG_StockSep AS p
inner join PRG_QuotationItem rbip 
on p.StockID=rbip.QUI_stock
GROUP BY rbip.QUI_QU_id) F
where F.tbiid=PRG_Quotation.QU_id) stocktitle, 
                      CASE PRG_Quotation.QU_type WHEN 0 THEN 'ثبت شده' WHEN 1 THEN 'تاييد نهايي' WHEN 3 THEN 'ابطال شده' WHEN 2 THEN 'تاييد مالي' END AS QU_type, PRG_Users_1.Name AS confirmer, 
                      dbo.mtsh(CONVERT(date, PRG_Quotation.QU_confirmdate)) AS confirmdate, PRG_Quotation.QU_desc,                      
                      PRG_Quotation.QU_QuotationNumberSP,PRG_RequestBuy.RB_DescNo,case Qu_DeliveryItem when 1 then 'تحویل شده' else '' end QuDeliveryItemStr ,
                      dbo.ArabicPersion(locostan.title) locostan,dbo.ArabicPersion(loccity.title) loccity,dbo.ArabicPersion(locCountry.title ) locCountry,QU_Discount,QU_addition,QU_tax,QU_duty
 ,QU_Note,PRG_Quotation.QU_cst_id FROM PRG_Quotation LEFT OUTER JOIN
                      PRG_RequestBuy ON PRG_Quotation.QU_rb_id = PRG_RequestBuy.RB_id INNER JOIN
                      PRG_Customers ON PRG_Quotation.QU_cst_id = PRG_Customers.cst_id INNER JOIN
                      PRG_Users ON PRG_Quotation.QU_creator = PRG_Users.USR_id LEFT OUTER JOIN
                      PRG_Loockup ON PRG_Quotation.QU_TypeTasfie = PRG_Loockup.LKP_id LEFT OUTER JOIN
                      PRG_Customers AS PRG_Customers_1 ON PRG_Quotation.QU_Tamin = PRG_Customers_1.cst_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_1 ON PRG_Quotation.QU_confirmer = PRG_Users_1.USR_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_2 ON PRG_Quotation.QU_SalesExpert = PRG_Users_2.USR_id LEFT JOIN
                      PRG_Dl dlsep ON PRG_Customers.csT_dl = dlsep.DLId inner join PRG_Location loccity on loccity.locationid=PRG_Customers.cst_city
                       inner join  PRG_Location locostan on locostan.locationid=PRG_Customers.cst_ostan
 Left join PRG_Location locCountry on locCountry.locationid=PRG_Customers.cst_Country
                     where PRG_Quotation.QU_id = " + Formid;
                    _DA.Fill(_DS);
                    dtleave = _DS.Tables[0];
                    //محاسبه بدهی مشتری
                    DataTable dtremind2 = new DataTable();
                    try
                    {
                        _DS = new DataSet();
                        _DA = new SqlDataAdapter();
                        SqlCommand Command2 = new SqlCommand(@"select 0 LKP_id,'صورت حساب مشتري' LKP_name,'[11]' LKP_value,cast(null as decimal) value,'' value2,'' Desccondition  ,cast(0 as decimal) DescValueFrom ,cast(0 as decimal) DescValueTO ,cast(0 as decimal) DescPriceFrom ,cast(0 as decimal) DescPriceTO,'' Desstatecredit ", cp2.connect());
                        try { if (cp2.connect().State == System.Data.ConnectionState.Closed) cp2.connect().Open(); }
                        catch { }
                        Command2.CommandType = CommandType.Text;
                        _DA.SelectCommand = Command2;
                        _DA.Fill(_DS);
                        dtremind2 = _DS.Tables[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //CheckMAli(dtleave.Rows[0]["QU_cst_id"].ToString(), dtremind2);
                    bedehi = dtremind2.Rows[0]["value"].ToString();
                    string ddd = dtremind2.Rows[0]["value"].ToString();
                    string ddd2 = dtleave.Rows[0]["QU_NetPrice"].ToString();
                    if (!string.IsNullOrEmpty(dtremind2.Rows[0]["value"].ToString()) & !string.IsNullOrEmpty(dtleave.Rows[0]["QU_NetPrice"].ToString()))
                    {
                        bedehi2 = decimal.Parse(dtremind2.Rows[0]["value"].ToString()) + (decimal.Parse(dtleave.Rows[0]["QU_NetPrice"].ToString())).ToString();
                    }
                }

                byte[] BytePdf = null;
                DataTable DtSmsLinkPdf;
                DataTable dtListForsat = _DS.Tables[0];
                DataTable dtlist;
                string Linkid = "";
                DataTable dtlistTellnumber = new DataTable();
                foreach (DataRow dr in dtSmsFormat.Rows)
                {
                    string StrTExt = dr["DFS_text"].ToString();

                    string matn = "";
                    if (formid1 != "8")// راننده قبلی
                    {
                        if (str != "")
                            str2 = Int32.Parse(str);
                    }
                    try
                    {
                        DtSmsLinkPdf = cp2.lindPdfSms(StrTExt, type, Formid);//دریافت لینک فایل پی دی اف
                        LinkPdf = DtSmsLinkPdf.Rows[0]["Linkpdf"].ToString();
                        Linkid = DtSmsLinkPdf.Rows[0]["LinkIdpdf"].ToString();
                        BytePdf = (byte[])DtSmsLinkPdf.Rows[0]["BytePdf"];
                    }
                    catch { }
                    string LinkPdf2 = "";
                    if (!LinkPdf.StartsWith("http"))
                        LinkPdf2 = "<<LinkPdf>>";
                    else
                        LinkPdf2 = LinkPdf;

                    if (type == "1")//درخواست خرید
                    {
                        matn = StrTExt.Replace("<<DateRequest>>", dtleave.Rows[0]["datee"].ToString()).Replace("<<NumberRequest>>", dtleave.Rows[0]["RB_no"].ToString()).Replace("<<CustomerRequest>>",
dtleave.Rows[0]["lname"].ToString()).Replace("<<stocktitle>>", dtleave.Rows[0]["stocktitle"].ToString()).Replace("<<TaminRequest>>", dtleave.Rows[0]["tamin"].ToString()).
Replace("<<VasetRequest>>", dtleave.Rows[0]["Vaset"].ToString()).Replace("<<typebuyRequest>>", dtleave.Rows[0]["typebuy"].ToString()).
Replace("<<PeriodRequest>>", dtleave.Rows[0]["LKP_name"].ToString()).Replace("<<valueRequest>>", dtleave.Rows[0]["RBIvalue"].ToString()).
Replace("<<valueRequestSecondary>>", dtleave.Rows[0]["RBIvalueSecondary"].ToString()).Replace("<<ConfimerRequest>>", dtleave.Rows[0]["confirmer"].ToString()).
Replace("<<CreatorRequest>>", dtleave.Rows[0]["Name"].ToString()).Replace("<<SaleExport>>", dtleave.Rows[0]["kasrshenas"].ToString()).
Replace("<<StateReq>>", dtleave.Rows[0]["RB_type"].ToString()).Replace("<<ConfimerDate>>", dtleave.Rows[0]["RB_confirmdate"].ToString()).
Replace("<<City>>", dtleave.Rows[0]["loccity"].ToString()).Replace("<<Ostan>>", dtleave.Rows[0]["locostan"].ToString())
.Replace("<<Country>>", dtleave.Rows[0]["locCountry"].ToString()).Replace("<<RBImetraj>>", dtleave.Rows[0]["RBImetraj"].ToString())
.Replace("<<RBIcarton>>", dtleave.Rows[0]["RBIcarton"].ToString()).Replace("<<RBIpalet>>", dtleave.Rows[0]["RBIpalet"].ToString())
.Replace("<<typecar>>", dtleave.Rows[0]["typecar"].ToString()).Replace("<<ConfimerRequest>>", dtleave.Rows[0]["typecar"].ToString())
.Replace("<<SalesExpertMobile>>", dtleave.Rows[0]["SalesExpertMobile"].ToString()).Replace("<<LinkPdf>>", LinkPdf2);
                    }
                    if (type == "2")//پیش فاکتور
                    {
                        string ggg = cls_public.convertTextToMoneyStr(dtleave.Rows[0]["QU_NetPrice"].ToString());
                        string gg = dtleave.Rows[0]["QU_NetPrice"].ToString();
                        matn = StrTExt.Replace("<<StateQ>>", dtleave.Rows[0]["StateQ"].ToString()).Replace("<<DateQuention>>", dtleave.Rows[0]["dateQ"].ToString()).Replace("<<NumberQuention>>",
dtleave.Rows[0]["QU_no"].ToString()).Replace("<<NumberReqQuention>>", dtleave.Rows[0]["RB_no"].ToString()).Replace("<<CustomerName>>", dtleave.Rows[0]["customersname"].ToString()).
Replace("<<Taminkonandeh>>", dtleave.Rows[0]["taminkonandeh"].ToString()).Replace("<<DlCode>>", dtleave.Rows[0]["dl"].ToString()).
Replace("<<typesale>>", dtleave.Rows[0]["typesale"].ToString()).Replace("<<TypeTasfie>>", dtleave.Rows[0]["LKP_name"].ToString()).
Replace("<<NetPrice>>", cls_public.convertTextToMoneyStr(dtleave.Rows[0]["QU_NetPrice"].ToString())).Replace("<<QUIvalue>>", dtleave.Rows[0]["QUIvalue"].ToString()).
Replace("<<QUIvalueSecondary>>", dtleave.Rows[0]["QUIvalueSecondary"].ToString()).Replace("<<CreatorQu>>", dtleave.Rows[0]["creator"].ToString()).
Replace("<<karshenasQu>>", dtleave.Rows[0]["karshenas"].ToString()).Replace("<<StocktitleQu>>", dtleave.Rows[0]["stocktitle"].ToString())
.Replace("<<StateQuention>>", dtleave.Rows[0]["QU_type"].ToString()).Replace("<<Confirmer>>", dtleave.Rows[0]["confirmer"].ToString())
.Replace("<<ConfimerDate>>", dtleave.Rows[0]["confirmdate"].ToString())
.Replace("<<QUdesc>>", dtleave.Rows[0]["QU_desc"].ToString()).Replace("<<QuotationNumberSP>>", dtleave.Rows[0]["QU_QuotationNumberSP"].ToString()).Replace("<<QuDeliveryItem>>", dtleave.Rows[0]["QuDeliveryItemStr"].ToString()).
Replace("<<City>>", dtleave.Rows[0]["loccity"].ToString()).Replace("<<Ostan>>", dtleave.Rows[0]["locostan"].ToString())
.Replace("<<Country>>", dtleave.Rows[0]["locCountry"].ToString()).Replace("<<QUaddition>>", dtleave.Rows[0]["QU_addition"].ToString())
.Replace("<<QUduty>>", cls_public.convertTextToMoneyStr(dtleave.Rows[0]["QU_duty"].ToString())).Replace("<<QUtax>>", cls_public.convertTextToMoneyStr(dtleave.Rows[0]["QU_tax"].ToString())).Replace("<<QUDiscount>>",
cls_public.convertTextToMoneyStr(dtleave.Rows[0]["QU_Discount"].ToString()))
.Replace("<<SalesExpertMobile>>", dtleave.Rows[0]["SalesExpertMobile"].ToString())
.Replace("<<CustomersMoeny>>", cls_public.convertTextToMoneyStr(bedehi))
.Replace("<<CustomersMoenyQU>>", cls_public.convertTextToMoneyStr(bedehi2))
.Replace("<<QUNote>>", dtleave.Rows[0]["Qu_Note"].ToString()).Replace("<<LinkPdf>>", LinkPdf2);

                    }

                    _DS = new DataSet();
                    _DA = new SqlDataAdapter();
                    Command = new SqlCommand("", cp2.connect());
                    _DA.SelectCommand = Command;
                    Command.CommandType = CommandType.Text;

                    if (!string.IsNullOrEmpty(str))
                    {
                        if (type == "1" || type == "12")//درخواست خرید
                            Command.CommandText = "SELECT tel_tell Tellnumber  FROM PRG_Tell where tel_cst_id=" + str;
                        if (type == "2" || type == "13")//پیش فاکتور
                            Command.CommandText = "SELECT tel_tell Tellnumber  FROM PRG_Tell where tel_cst_id=" + str;
                    }

                    _DA.Fill(_DS);
                    dtlistTellnumber = _DS.Tables[0];
                    if (dtlistTellnumber.Rows.Count > 0)
                    {
                        if (dtlistTellnumber.Rows[0]["Tellnumber"].ToString() != "")
                        {
                            if (dtlistTellnumber.Rows[0]["Tellnumber"].ToString().Length == 11)
                            {
                                if (dtlistTellnumber.Rows[0]["Tellnumber"].ToString().StartsWith("0"))
                                {
                                    if (!StrTExt.Contains("<<LinkPdf>>") || LinkPdf.StartsWith("http"))
                                    {
                                        DtSendSmsClass.Rows.Add(1, 0, DBNull.Value, "LinkPdf:" + LinkPdf);
                                    }
                                    ///////////////////////////////////////////////////////////////////////////////////////////
                                    if (DtSendSmsClass != null)
                                    {
                                        Nullable<int> formidInt = null;
                                        if (!string.IsNullOrEmpty(Formid))
                                        {
                                            try
                                            {
                                                formidInt = Int32.Parse(Formid);
                                            }
                                            catch { }
                                        }
                                        if (DtSendSmsClass.Rows.Count > 0)
                                        {
                                            DtValidSMS.Rows.Add(matn, 0, str2, DBNull.Value, Int32.Parse(type), dtsend.Date, TimeJob, dtlistTellnumber.Rows[0]["Tellnumber"].ToString()
                       , DBNull.Value, linenumber, DBNull.Value, formidInt, Linkid, BytePdf);
                                        }
                                        //classes.clsGlobal.AlarmSave(StringSmsReaualt);
                                    }
                                    //////////////////////////////////////////////////////////////////////////////////////////////////////////
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }

            }
            if (DtValidSMS.Rows.Count > 0)
            {

                SqlCommand command = new SqlCommand("", cp2.connect());
                command.CommandText = "SPInSendSmsChart";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@TableSendSmsChart", DtValidSMS);
                command.Parameters.Add("@TypeCompany", SqlDbType.TinyInt).Value = 1;
                command.Parameters.Add("@st", SqlDbType.Bit).Value = 0;
                command.Parameters.Add("@USR", SqlDbType.Int).Value = DBNull.Value;
                command.CommandType = CommandType.StoredProcedure;
                try
                {
                    try { if (cp2.connect().State == System.Data.ConnectionState.Closed) cp2.connect().Open(); }
                    catch { }
                    command.ExecuteNonQuery();
                    try { if (cp2.connect().State == System.Data.ConnectionState.Open) cp2.connect().Close(); }
                    catch { }
                }
                catch (Exception ex)
                {
                }
            }
        }


        public DataTable lindPdfSms(string StrTExt, string formid, string scalerReqID2)
        {

            string URL = "";
            DataTable dt_Link = select_command("select TNZ_sharh from PRG_Propertise where TNZ_name='WebServeSendLinkPrint'");
            DataTable DtShowSignsPrintLadingType = select_command("select TNZ_sharh from PRG_Propertise where TNZ_name='ShowSignsPrintLadingType'");
            DataTable DtWebServeSendLinkPrint = select_command("select TNZ_sharh from PRG_Propertise where TNZ_name='WebServeSendLinkPrint'");

            DataTable DtSmsLinkPdf = new DataTable();
            DtSmsLinkPdf.Columns.Add("Linkpdf", typeof(string));
            DtSmsLinkPdf.Columns.Add("LinkIdpdf", typeof(string));
            DtSmsLinkPdf.Columns.Add("BytePdf", typeof(byte[]));
            string LinkPdf = "";
            if (string.IsNullOrEmpty(DtWebServeSendLinkPrint.Rows[0][0].ToString()))
                URL = "http://85.185.161.223:5488";
            else
                URL = DtWebServeSendLinkPrint.Rows[0][0].ToString();
            if (StrTExt.Contains("LinkPdf"))
            {
                if (formid == "1" || formid == "2" || formid == "3")
                {
                    DataTable dt_Pdf = select_command("select DR_script,DR_existence,DR_ReportData from dbo.PRG_DynamicReport where DR_ShowPortal =1 AND DR_existence = " + formid);
                    if (dt_Pdf.Rows.Count > 0)
                    {
                        DataSet _DS = new DataSet();
                        SqlDataAdapter _DA = new SqlDataAdapter();
                        SqlCommand Command2 = new SqlCommand(dt_Pdf.Rows[0]["DR_script"].ToString(),_con);
                        Command2.Parameters.Add("@id", SqlDbType.Int).Value = Int32.Parse(scalerReqID2);
                        try { if (_con.State == System.Data.ConnectionState.Closed) _con.Open(); }
                        catch { }
                        Command2.CommandType = CommandType.Text;
                        _DA.SelectCommand = Command2;
                        _DA.Fill(_DS);
                        DataTable dtprint = _DS.Tables[0];
                        if (dtprint.Rows.Count > 0)
                        {
                            // DataTable dtprint = classes.clsDB.executedQuery(dt_Pdf.Rows[0]["DR_script"].ToString());
                            foreach (DataRow dr in dtprint.Rows)
                            {
                                try
                                {
                                    string num2str = CrmController.num2sr(dr[dr["num2sr"].ToString()].ToString());
                                    dr["num2sr"] = num2str;
                                    dr.EndEdit();
                                    dr.AcceptChanges();
                                }
                                catch { }
                            }
                            DataTable dtremind = new DataTable();
                            DataTable Dtsign = new DataTable();
                            FastReport.Report report = new FastReport.Report();
                            if (formid == "3")
                            {
                                if (DtShowSignsPrintLadingType.Rows[0][0].ToInt() == 0)
                                    Dtsign = GetSignerForm(report, formid.ToInt(), scalerReqID2);//چاپ امضاها
                                if (DtShowSignsPrintLadingType.Rows[0][0].ToInt() == 1)
                                {
                                    DataTable DtfindId = select_command(@"select LD_QU_id from Prg_Lading where LD_id=" + scalerReqID2.ToString());

                                    Dtsign = GetSignerForm(report, 2, DtfindId.Rows[0][0].ToString());//پیش فاکتور
                                }
                                if (DtShowSignsPrintLadingType.Rows[0][0].ToInt() == 2)
                                {
                                    DataTable DtfindId = select_command(@"select LD_INV_id from Prg_Lading where LD_id=" + scalerReqID2.ToString());

                                    Dtsign = GetSignerForm(report, 6, DtfindId.Rows[0][0].ToString());//پیش فاکتور
                                }
                            }
                            else
                                Dtsign = GetSignerForm(report, formid.ToInt(), scalerReqID2.ToString());//چاپ امضاها
                            DataRow dt_Pdfdr = dt_Pdf.Rows[0];
                            CrmController.DaynamicReportDatabase(dt_Pdfdr, dtprint, Dtsign);

                            byte[] BytePdf = CrmController.sendfile;
                            CrmController.sendfile = null;
                            //DaynamicReportDatabase(dt_Pdfdr, dtprint, Dtsign);
                            Random rand = new Random();
                            string crand = rand.Next(1111, 9999).ToString();

                            string localurl = scalerReqID2 + "-" + crand;

                            string web_url = "";
                            if (!string.IsNullOrEmpty(localurl) && !localurl.StartsWith("Error"))
                            {
                                web_url = "http://" + URL + "/api/pdf/infoByte/" + localurl;
                            }
                            DtSmsLinkPdf.Rows.Add(web_url, localurl, BytePdf);

                            //if (string.IsNullOrEmpty(DtWebServeSendLinkPrint.Rows[0][0].ToString()))
                            //    URL = DtWebServeSendLinkPrint.Rows[0][0].ToString();

                            //RestClient client = new RestClient(FrmMain.WebServeSendLinkPrint);
                            //RestRequest client_request = new RestRequest("/api/pdf/addByte", Method.POST);
                            //var requestModel = new
                            //{
                            //    localurl = scalerReqID2 + "-" + crand,
                            //    BytePdf = BytePdf
                            //};

                            //string jsonBody = JsonConvert.SerializeObject(requestModel);

                            //client_request.AddParameter(
                            //    "application/json",
                            //    jsonBody,
                            //    ParameterType.RequestBody
                            //);
                            ////client_request.AddParameter("localurl", scalerReqID2 + "-" + crand);
                            ////client_request.AddParameter("BytePdf", BytePdf);
                            //var response = client.Execute(client_request);
                            //string json = response.Content;
                            //try
                            //{
                            //    string correctedJson = json.Replace("\\\"", "\"")
                            //                   .Replace("\\\\", "\\");
                            //    correctedJson = correctedJson.Remove(0, 1);
                            //    correctedJson = correctedJson.Substring(0, correctedJson.Length - 1);
                            //    cls_response_addpdf data = JsonConvert.DeserializeObject<cls_response_addpdf>(correctedJson);
                            //    if (data.success)
                            //    {
                            //        LinkPdf = data.url.weburl;
                            //    }
                            //    else
                            //    {
                            //        //MessageBox.Show(data.message);
                            //    }
                            //}
                            //catch (Exception ex)
                            //{ }
                        }
                    }
                }
            }
            return DtSmsLinkPdf;
        }

        public bool update(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            var result = 0;

            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                //_command = new SqlCommand(StoredProcedure, _con);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                foreach (SqlParameter parameter in parameters)
                {
                    _command.Parameters.Add(parameter);
                }
                try
                {
                    // Classes.Library.WriteErrorLog("100:");
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                            //Classes.Library.WriteErrorLog("101:");

                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    //Classes.Library.WriteErrorLog("102:");

                    result = _command.ExecuteNonQuery();
                    CloseConnectionIfIdle(transaction);
                }
                catch (Exception ex)
                {
                    if (transaction == null)
                        _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    Console.WriteLine(ex.Message);
                }
            }
            return result > 0;
        }

        public bool updateDatatable(string StoredProcedure, SqlParameter[] parameters, string DtNameDatabase, DataTable Dt, SqlTransaction transaction = null)
        {
            var result = 0;

            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                //_command = new SqlCommand(StoredProcedure, _con);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                //_command.Parameters.Clear();
                _command.Parameters.AddWithValue(DtNameDatabase, Dt);
                foreach (SqlParameter parameter in parameters)
                {
                    _command.Parameters.Add(parameter);
                }
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();

                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    _command.ExecuteNonQuery();
                    result = 2;
                    CloseConnectionIfIdle(transaction);
                }
                catch (Exception ex)
                {
                    if (transaction == null)
                        _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                    Console.WriteLine(ex.Message);
                }
            }
            return result > 0;
        }

        public bool delete(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            bool result = false;
            using (SqlCommand _command = new SqlCommand(StoredProcedure, _con))
            {
                //_command = new SqlCommand(StoredProcedure, _con);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                foreach (SqlParameter parameter in parameters)
                {
                    _command.Parameters.Add(parameter);
                }
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }

                    result =Convert.ToBoolean( _command.ExecuteScalar());

                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                }
            }
            return result;//> 0;
        }
        public bool command(string command, SqlTransaction transaction = null) // تابع insert به همراه دریافت لیست پارامتر
        {
            var result = 0;
            using (SqlCommand _command = new SqlCommand(command, _con))
            {
                //_command = new SqlCommand(command, _con);
                _command.CommandType = CommandType.Text;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                _command.Parameters.Clear();
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    result = _command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                }
            }
            return result > 0;
        }
        public bool commandBool(string command, SqlTransaction transaction = null) // تابع insert به همراه دریافت لیست پارامتر
        {
            bool result = false;
            using (SqlCommand _command = new SqlCommand(command, _con))
            {
                //_command = new SqlCommand(command, _con);
                _command.CommandType = CommandType.Text;
                _command.CommandTimeout = CommandTimeoutSeconds;
                _command.Transaction = transaction;
                _command.Parameters.Clear();
                try
                {
                    if (_con.State != ConnectionState.Open)
                    {
                        try
                        {
                            _con.Open();
                        }
                        catch
                        {
                            sqlError.success = false;
                        }
                    }
                    result = Convert.ToBoolean(_command.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    _con.Close();
                    if (sqlError.success)
                        sqlError.message = ex.Message;
                }
            }
            return result;
        }
        public int? max_id(string Table, string Column_id = "id")
        {
            DataTable dt = new DataTable();
            _dataAdapter = new SqlDataAdapter($"select Max({Column_id}) from {Table}", _con);
            try
            {
                _dataAdapter.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToInt() + 1;
                }
                else
                    return 1;
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
            }
            return null;
        }
    }
}
