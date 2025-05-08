using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft;
using System.IO.Compression;
using sigma_api.ModelView;

namespace sigma_api.Connection
{
    public class sql_db : messagebase
    {
        public static bool enterFlagtest = false;
        public static string servername = "";
        public static string usersa = "";
        public static string passsa = "";
        public static string databasename = "";
        public static string PortName = "";

        public static void DecompressFile(string sourceFile, string destinationFile)
        {
            // make sure the source file is there
            if (System.IO.File.Exists(sourceFile) == false)
                throw new FileNotFoundException();

            // Create the streams and byte arrays needed
            FileStream sourceStream = null;
            FileStream destinationStream = null;
            GZipStream decompressedStream = null;
            byte[] quartetBuffer = null;

            try
            {
                // Read in the compressed source stream
                sourceStream = new FileStream(sourceFile, FileMode.Open);

                // Create a compression stream pointing to the destiantion stream
                decompressedStream = new GZipStream(sourceStream, CompressionMode.Decompress, true);

                // Read the footer to determine the length of the destiantion file
                quartetBuffer = new byte[4];
                int position = (int)sourceStream.Length - 4;
                sourceStream.Position = position;
                sourceStream.Read(quartetBuffer, 0, 4);
                sourceStream.Position = 0;
                int checkLength = BitConverter.ToInt32(quartetBuffer, 0);

                byte[] buffer = new byte[checkLength + 100];

                int offset = 0;
                int total = 0;

                // Read the compressed data into the buffer
                while (true)
                {
                    int bytesRead = decompressedStream.Read(buffer, offset, 100);

                    if (bytesRead == 0)
                        break;

                    offset += bytesRead;
                    total += bytesRead;
                }

                // Now write everything to the destination file
                destinationStream = new FileStream(destinationFile, FileMode.Create);
                destinationStream.Write(buffer, 0, total);

                // and flush everyhting to clean out the buffer
                destinationStream.Flush();
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Make sure we allways close all streams
                if (sourceStream != null)
                    sourceStream.Close();

                if (decompressedStream != null)
                    decompressedStream.Close();

                if (destinationStream != null)
                    destinationStream.Close();
            }
        }
        private static string readConnectFile()
        {
            SqlConnection sqlConnection;
            string path = "", data = "", SNLARG = "";
            bool check = false;
            string connectionString = null;
            if (File.Exists("Connect.txt"))
            {
                string[] filee = File.ReadAllLines("Connect.txt");
                string HostName = filee[0];
                string Username = filee[1];
                string Password = filee[2];
                string DbName = filee[3];
                connectionString = "Data Source=" + HostName + "; Initial Catalog=" + DbName  + "; User ID=" + Username + "; Password=" + Password + ";";
                sqlConnection = new SqlConnection(connectionString);
                try
                {
                    sqlConnection.Open();
                    sqlConnection.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ارتباط با ديتابيس مقدور نمي باشد");
                }
            }
            else
            {
                Console.WriteLine("فايل اتصال به ديتابيس در مسير اجراي برنامه وجود ندارد");
            }
            return connectionString;

            /* try
             {
                 path = "Connect.txt";
                 DecompressFile("Connect", path);
                 TextReader reader1 = new StreamReader(path);
                 data = reader1.ReadToEnd();
                 reader1.Close();
             }
             catch (Exception ex)
             {
                 Console.WriteLine("فايل اتصال به ديتابيس در مسير اجراي برنامه وجود ندارد");
                 check = true;
             }
             if (!check)
             {
                 bool checkvojod = false;
                 try
                 {
                     System.IO.File.Delete(path);
                 }
                 catch
                 {
                 }
                 string[] filee = data.Split('\r');
                 PortName = filee[4];
                 databasename = filee[3];
                 SNLARG = filee[0].ToUpper();
                 servername = SNLARG;
                 if (filee[4] != "")
                     filee[4] = "," + filee[4];
                 else
                     filee[4] = "";
                 connectionString = "Data Source=" + filee[0] + filee[4] + "; Initial Catalog=" + filee[3] + "; User ID=" + filee[1] + "; Password=" + filee[2] + ";";

                 sqlConnection = new SqlConnection(connectionString);
                 try
                 {
                     sqlConnection.Open();
                     sqlConnection.Close();

                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine("ارتباط با ديتابيس مقدور نمي باشد");
                 }

             }
             return connectionString;*/
        }
        protected SqlConnection connct => new SqlConnection(connection);

        private string connection => readConnectFile();

    }
}
