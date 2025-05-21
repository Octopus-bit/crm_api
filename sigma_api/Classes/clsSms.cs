using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using sigma_api.SmsNegarModels;

namespace sigma_api.Classes
{
    public class clsSms
    {
        public static bool IsSend = false;
        public static string returnCode = "";
        public static String GetTimestamp(System.DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public static async void send(string Number, string Text)
        {
            IsSend = false;
            string linenumber = "";
            int PAnelSms = clsProperties.GetPropertise("PAnelSms").ToInt();
            if (PAnelSms == 2)
            {
                linenumber = clsProperties.GetPropertise("smsirphon1").ToString(); 
            }
            if (PAnelSms == 1)
            {
                linenumber = clsProperties.GetPropertise("txtnumbersms").ToString(); 
            }
            if (PAnelSms == 3)
            {
                linenumber = clsProperties.GetPropertise("wideirphon1").ToString(); 
            }

            if (PAnelSms == 2)
            {
                String timeStamp = GetTimestamp(new cls_public().GetDateTime().AddMinutes(10));
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("x-api-key", clsProperties.GetPropertise("ApiTockenSmsir").ToString());
                var payload = @"{" + "\n" +
                @" ""lineNumber"": " + linenumber + "," + "\n" +
                @" ""messageTexts"": [" + "\n" +
                @" """ + Text + @"""" + "\n" +
                @" ]," + "\n" +
                @" ""mobiles"": [" + "\n" +
                @" """ + Number + @"""" + "\n" +
                @" ]," + "\n" +
                @" ""sendDateTime"": null" + "\n" +
                @"}";
                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://api.sms.ir/v1/send/likeToLike", content);
                var result = await response.Content.ReadAsStringAsync();
                string[] str1 = result.Split(',');
                string[] str22 = str1[1].Split(':');
                bool check = false;
                foreach (string strc1 in str1)
                {
                    str22 = strc1.Split(':');
                    if (str22[0].ToString() == "\"status\"")
                    {
                        if (str22[1].ToString() == "1")
                        {
                            check = true;
                        }
                    }
                }
                if (check)
                {
                    foreach (string strc11 in str1)
                    {
                        str22 = strc11.Split(':');
                        if (str22[0].ToString() == "\"messageIds\"")
                        {
                            returnCode = str22[1].ToString().Substring(1, str22[1].Length - 2);
                            IsSend = true;
                            //DtSendSmsClass.Rows.Add(1, 1, returnCode, DBNull.Value);
                        }
                    }
                }
                else
                {
                    foreach (string strc11 in str1)
                    {
                        str22 = strc11.Split(':');
                        if (str22[0].ToString() == "\"message\"")
                        {
                            try
                            {
                                returnCode = str22[1].ToString().Substring(1, str22[1].Length - 2);
                            }
                            catch
                            {
                                returnCode = str22[1].ToString();
                            }
                            IsSend = false;
                            //DtSendSmsClass.Rows.Add(1, 0, returnCode, returnCode);
                        }
                    }
                }

            }
            if (PAnelSms == 1)
            {
                string path = "https://mehrafraz.com/fullrest/";
                var model = new SendModel();
                model.UserName = clsProperties.GetPropertise("txtusersms").ToString();
                model.Password = clsProperties.GetPropertise("txtpasssms").ToString();
                model.DomainName = clsProperties.GetPropertise("txtdomainsms").ToString();
                model.Smsbody = Text;
                model.Mobiles = new string[] { Number };
                model.SenderNumber = linenumber;
                model.Id = new Random().Next(999999).ToString();
                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = path + "/api/Send";
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(url, data).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<ResponseCodes>(responseContent);
                        if (result.Status == 0)
                        {
                            IsSend = true;
                        }
                    }

                }
            }
            if (PAnelSms == 3)
            {
                IsSend = false;
                //if (clsProperties.GetPropertise("").ToString()FrmMain.WideirUsername != "" & clsProperties.GetPropertise("").ToString()FrmMain.WideirPass != "")
                //{
                //    var ObjMyService = new ServiceReference.BoxServiceSoapClient();
                //    ServiceReference3.BoxServiceSoapClient BoxServiceV5 = new ServiceReference3.BoxServiceSoapClient();
                //    ArrayList arlist = new ArrayList();
                //    ServiceReference3.ArrayOfString ArrayOfString2 = new ServiceReference3.ArrayOfString();
                //    ServiceReference3.ArrayOfString ArrayOfString5 = new ServiceReference3.ArrayOfString();
                //    ArrayOfString2.Add(dtlistTellnumber.Rows[0]["Tellnumber"].ToString());
                //    try
                //    {
                //        ArrayOfString5 = ObjMyService.SendMessage(FrmMain.WideirUsername, FrmMain.WideirPass, FrmMain.wideirphon1, ArrayOfString2, matn, "0");
                //        try
                //        {
                //            if (Int64.Parse(ArrayOfString5[0].ToString()) > 0)
                //            {
                //                checkreturncode = true;
                //                returnCode = ArrayOfString5[0].ToString();
                //                DtSendSmsClass.Rows.Add(1, 1, returnCode, DBNull.Value);
                //            }
                //        }
                //        catch
                //        {
                //            checkreturncode = false;
                //            DtSendSmsClass.Rows.Add(1, 0, "", ArrayOfString5[0].ToString());
                //        }
                //    }
                //    catch { }
                //}
            }
        }
    }
}
