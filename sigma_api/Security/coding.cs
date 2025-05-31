using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace sigma_api.Security
{
    public class coding
    {
        public static string EncryptString(string Text)
        {
            byte[] IV = new byte[8] { 0, 254, 100, 7, 17, 180, 255, 1 };
            string Key = "1^s~!w43#?,imani";
            byte[] buffer = Encoding.UTF8.GetBytes(Text);
            TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            triple.IV = IV;
            triple.Key = md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(Key));
            byte[] encodeText = triple.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length);
            return Convert.ToBase64String(encodeText);
        }


        public static string DecryptString(string EncryptText)
        {
            byte[] IV = new byte[8] { 0, 254, 100, 7, 17, 180, 255, 1 };
            string Key = "1^s~!w43#?,imani";
            byte[] decodeText = null;
            try
            {
                byte[] buffer = Convert.FromBase64String(EncryptText);
                TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                triple.IV = IV;
                triple.Key = md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(Key));
                decodeText = triple.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length);
            }
            catch
            {
                return EncryptText;
            }
            return Encoding.UTF8.GetString(decodeText);
        }

        public static string SafeFarsiStr(string input)
        {
            return input.Replace("ي", "ی").Replace("ك", "ک");
        }

        public static string SigmaEncryptString(string strToEncrypt)
        {
            if (string.IsNullOrEmpty(strToEncrypt))
                return "";
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            return System.Text.RegularExpressions.Regex.Replace
                (BitConverter.ToString(hashBytes), "-", "").ToLower();
        }
    }
}
