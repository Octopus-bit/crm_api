using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using sigma_api.ModelView;


namespace sigma_api.Classes
{
    public class cls_public : Connection.sql_db
    {
        SqlConnection _con;
        SqlDataAdapter _dataAdapter;
        SqlCommand _command;
        public static messagebase sqlError = new messagebase();
        public cls_public()
        {
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

        public DateTime GetDateTime(SqlTransaction transaction = null)
        {
            DateTime getdate = new DateTime();
            SqlCommand cmdLog = new SqlCommand("", _con);
            cmdLog.Transaction = transaction;
            cmdLog.CommandType = CommandType.Text;
            cmdLog.CommandText = "Select getdate()";
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    _con.Open();
                }

                string getdatee = cmdLog.ExecuteScalar().ToString();
                if (_con.State == ConnectionState.Open)
                {
                    _con.Close();
                }
                getdate = Convert.ToDateTime(getdatee);

            }
            catch
            {
                if (_con.State == ConnectionState.Open)
                {
                    _con.Close();
                }

            }
            finally
            {
                if (_con.State == ConnectionState.Open)
                {
                    _con.Close();
                }

            }
            return getdate;
        }
        public DataTable select(string StoredProcedure, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
            _command.Transaction = transaction;
            _dataAdapter = new SqlDataAdapter(_command);
            try
            {
                if (_con.State != ConnectionState.Open)
                {
                    _con.Open();
                }
                _command.ExecuteNonQuery();
                _dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
                return null;
            }
        }

        public DataTable select_row(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
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
                _command.ExecuteNonQuery();
                _dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
                return null;
            }
        }

        public DataTable select_command(string Select_command, SqlParameter[] parameters = null, SqlTransaction transaction = null)
        {
            DataTable dt = new DataTable();
            _command = new SqlCommand(Select_command, _con);
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
                if (sqlError.success)
                    sqlError.message = ex.Message;
                return null;
            }
        }

        public bool insert(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            var result = 0;
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
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
                result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
                throw;
            }
            return result > 0;
        }

        public string insert_getid(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            string id;
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
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
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
                return "0";
            }
            return id;
        }

        public bool update(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            var result = 0;
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
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
                result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
                Console.WriteLine(ex.Message);
            }
            return result > 0;
        }

        public bool delete(string StoredProcedure, SqlParameter[] parameters, SqlTransaction transaction = null)
        {
            var result = 0;
            _command = new SqlCommand(StoredProcedure, _con);
            _command.CommandType = CommandType.StoredProcedure;
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

                result = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (sqlError.success)
                    sqlError.message = ex.Message;
            }
            return result > 0;
        }
        public bool command(string command, SqlTransaction transaction = null) // تابع insert به همراه دریافت لیست پارامتر
        {
            var result = 0;
            _command = new SqlCommand(command, _con);
            _command.CommandType = CommandType.Text;
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
                if (sqlError.success)
                    sqlError.message = ex.Message;
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
