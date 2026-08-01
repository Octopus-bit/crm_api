using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace sigma_api.Classes
{
    public static class Library
    {

        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
        public static void WriteErrorLogInventory(string Message)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\log\\Inventory.txt");
                }
                catch { }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\Inventory.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
        public static void WriteErrorLogCustomersLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                try
                {
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\log\\CustomersLog.txt");
                }
                catch { }
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\log\\CustomersLog.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
