using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.Graph.Models;
using sigma_api.ModelView;
using static iText.IO.Image.Jpeg2000ImageData;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


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
