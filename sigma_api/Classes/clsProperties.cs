using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sigma_api.Classes
{
    class clsProperties
    {
        public string[] provincePhoneCodes = new string[]
        {
            "041", // آذربایجان شرقی
            "044", // آذربایجان غربی
            "045", // اردبیل
            "031", // اصفهان
            "084", // ایلام
            "077", // بوشهر
            "021", // تهران
            "038", // چهارمحال و بختیاری
            "051", // خراسان رضوی
            "056", // خراسان جنوبی
            "058", // خراسان شمالی
            "061", // خوزستان
            "024", // زنجان
            "023", // سمنان
            "054", // سیستان و بلوچستان
            "071", // فارس
            "026", // البرز
            "025", // قم
            "028", // قزوین
            "087", // کردستان
            "034", // کرمان
            "083", // کرمانشاه
            "074", // کهگیلویه و بویراحمد
            "017", // گلستان
            "013", // گیلان
            "066", // لرستان
            "011", // مازندران
            "086", // مرکزی
            "076", // هرمزگان
            "081", // همدان
            "035"  // یزد
        };

        private static DataTable dtpropertis = new DataTable();

        public static object GetPropertise(string name) // خواندن مقادیر جدول Properties 
        {
            if (dtpropertis.Rows.Count <= 0)
            {
                cls_public cp = new cls_public();
                dtpropertis = cp.select_command("SELECT * FROM dbo.PRG_Propertise WHERE ISNULL(TNZ_usr_id,-1) = -1");
            }

            DataView dv = new DataView(dtpropertis);
            dv.RowFilter = $"TNZ_name = '{name}'";
            var data = dv.ToTable().Rows[0]["TNZ_image"].ToString() == null ?dv.ToTable().Rows[0]["TNZ_sharh"]: dv.ToTable().Rows[0]["TNZ_image"];

            return data;
            /*foreach (DataRow row4 in dtpropertis.Rows)
            {
                hhh++;

                if (row4["TNZ_name"].ToString() == "Kiosk_SaleType")
                {
                    try
                    {
                        Kiosk_SaleType = row4["TNZ_sharh"].ToInt();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Kiosk_SaleUserId")
                {
                    try
                    {
                        Kiosk_SaleUserId = row4["TNZ_sharh"].ToInt();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Kiosk_TerminalId")
                {
                    try
                    {
                        Kiosk_TerminalId = row4["TNZ_sharh"].ToInt();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Kiosk_BarcodeModel")
                {
                    try
                    {
                        Kiosk_BarcodeModel = row4["TNZ_sharh"].ToInt();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DriveX")
                {
                    try
                    {
                        DriveX = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtaddgmail")
                {
                    try
                    {
                        txtaddgmail = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtnameforoshgah")
                {
                    try
                    {
                        txtnameforoshgah = namecompany = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "textpassgmail")
                {
                    try
                    {
                        txtpassgmail = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "daybargiri")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            daybargiri = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            daybargiri = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TimeWork")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            TimeWork = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            TimeWork = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "timesendbarelamnashode")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            timesendbarelamnashode = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            timesendbarelamnashode = "";
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "DayInMounth")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            DayInMounth = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            DayInMounth = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TxtTelorans")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            TxtTelorans = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            TxtTelorans = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ServerIpPort")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            ServerIpPort = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            ServerIpPort = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "dayreport")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            dayreport = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            dayreport = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Countcoding")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            Countcoding = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            Countcoding = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Medal")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            Medal = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            Medal = 0;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WeightL")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            WeightL = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            WeightL = 10;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WeightR")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            WeightR = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            WeightR = 10;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "WeightF")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            WeightF = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            WeightF = 10;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WeightM")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            WeightM = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            WeightM = 10;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "IPVoip")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            IPVoip = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            IPVoip = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "PassVoip")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            PassVoip = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            PassVoip = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "UserVoip")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            UserVoip = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            UserVoip = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TimeoutVoip")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            TimeoutVoip = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            TimeoutVoip = 0;
                        }
                    }
                    catch
                    {
                    }
                }
                //else if (row4["TNZ_name"].ToString() == "NumberVoipSend")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            NumberVoipSend = row4["TNZ_sharh"].ToString();
                //        }
                //        else
                //        {
                //            NumberVoipSend = "";
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                else if (row4["TNZ_name"].ToString() == "countgheymat")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            countgheymat = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            countgheymat = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "whatsupstock")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            whatsupstock = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            whatsupstock = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "DataMali")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            DataMali = row4["TNZ_sharh"].ToString();
                            FrmMain.DataMali = DataMali;
                            //classes.clsGlobal.Stringreplace();
                            //classes.clsDB.Stringreplaceclsdb();
                        }
                        else
                        {
                            DataMali = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "ServerName")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            ServerName = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            ServerName = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "whatsupcustomers")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            whatsupcustomers = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            whatsupcustomers = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "matnbargiricar")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            matnbargiricar = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            matnbargiricar = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "matnelambarnashode")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            matnelambarnashode = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            matnelambarnashode = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "matnbargiricustomer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            matnbargiricustomer = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            matnbargiricustomer = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "matnbargiriBarbari")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            matnbargiriBarbari = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            matnbargiriBarbari = null;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "shurtkeyCustomer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            shurtkeyCustomer = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            shurtkeyCustomer = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "shurtkeyRest")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            shurtkeyRest = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            shurtkeyRest = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "desc1customer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            desc1customer = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            desc1customer = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "desc2customer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            desc2customer = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            desc2customer = null;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "chkitemcode")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkitemcode = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkitemcode = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AutomaticQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkAutomaticQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkAutomaticQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "printsystemQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkprintsystemQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkprintsystemQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "notchangeQu")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            notchangeQu = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            notchangeQu = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AverageMoneyRas")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            AverageMoneyRas = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            AverageMoneyRas = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DayInMounthChkRas")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DayInMounthChkRas = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DayInMounthChkRas = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DateNowRas")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DateNowRas = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DateNowRas = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "GpsLocation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            GpsLocation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            GpsLocation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SoundRecord")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SoundRecord = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SoundRecord = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "OkcustomerQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            OkcustomerQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            OkcustomerQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chkaddsizepricing")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkaddsizepricing = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkaddsizepricing = false;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "janeshin")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkjaneshin = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkjaneshin = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Chequereq")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkChequereq = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkChequereq = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "etebarreq")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chketebarreq = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chketebarreq = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SubunitQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SubunitQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SubunitQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DiscontDetailQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DiscontDetailQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DiscontDetailQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "QnoRequestnuy")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            QnoRequestnuy = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            QnoRequestnuy = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DiscontQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DiscontQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DiscontQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AdditionQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            AdditionQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            AdditionQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AddFeeQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            AddFeeQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            AddFeeQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AddFeeQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            AddFeeQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            AddFeeQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "changeReQue")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkchangeReQue = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkchangeReQue = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "changeQueRe")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkchangeQueRe = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkchangeQueRe = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "addresscustomer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            addresscustomer = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            addresscustomer = false;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "Autotaxduty")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            Autotaxduty = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            Autotaxduty = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TaxQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TaxQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TaxQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "DutyQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DutyQuotation = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DutyQuotation = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DescQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            DescQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            DescQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "KolQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            KolQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            KolQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "expireddayQuotation")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            expireddayQuotation = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            expireddayQuotation = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "kalibre")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            kalibre = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "descunit1")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            descunit1 = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "descunit2")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            descunit2 = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TitleSaleGroup")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            TitleSaleGroup = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TitlePurchaseGroup")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            TitlePurchaseGroup = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ItemAdition1")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            ItemAdition1 = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ItemAdition2")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            ItemAdition2 = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "descunit3")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            descunit3 = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Notkalibre")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            Notkalibre = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "typetasvie")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            typetasvie = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            typetasvie = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SetUnitOrnot")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SetUnitOrnot = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SetUnitOrnot = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotShowUnitOrnot")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotShowUnitOrnot = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotShowUnitOrnot = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowSupplierItem")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowSupplierItem = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowSupplierItem = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "RemainRequestItemOld")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            RemainRequestItemOld = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            RemainRequestItemOld = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TransferNotQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TransferNotQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TransferNotQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotCheckSecendUnit")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotCheckSecendUnit = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotCheckSecendUnit = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotokA4Q")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotokA4Q = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotokA4Q = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "PricingQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            PricingQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            PricingQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotokA5Q")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotokA5Q = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotokA5Q = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "GroupStockNotShow")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            GroupStockNotShow = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            GroupStockNotShow = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowSupplierItemQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowSupplierItemQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowSupplierItemQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "notshowtamin")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            notshowtamin = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            notshowtamin = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "notshowvaset")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            notshowvaset = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            notshowvaset = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "anbardetail")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkanbardetail = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkanbardetail = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chkdegree")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkdegree = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkdegree = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "holidaycall")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            holidaycall = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            holidaycall = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Onlyitemgroupstock")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            Onlyitemgroupstock = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            Onlyitemgroupstock = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "bargirinofactor")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            bargirinofactor = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            bargirinofactor = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "PmAutomaticBargiri")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            automaticbargirismscustomer = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            automaticbargirismscustomer = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "PmAutomaticBargiriCar")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            automaticbargirismscar = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            automaticbargirismscar = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chksize")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chksize = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chksize = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowtemDefualt")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowtemDefualt = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowtemDefualt = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "HaveTracing")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            HaveTracing = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            HaveTracing = false;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "chkbalanceQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkbalanceQ = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkbalanceQ = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chkbalanceR")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkbalanceR = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkbalanceR = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chkbalanceI")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkbalanceI = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkbalanceI = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "chkNegativebalance")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkNegativebalance = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkNegativebalance = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "CodingItem")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            CodingItem = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            CodingItem = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ItemCodePerson")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ItemCodePerson = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ItemCodePerson = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotshowWeight")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotshowWeight = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotshowWeight = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotshowTypecar")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotshowTypecar = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotshowTypecar = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotshowFinal")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotshowFinal = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotshowFinal = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotshowWork")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotshowWork = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotshowWork = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotchangeUnit2")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotchangeUnit2 = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotchangeUnit2 = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowSaleItem")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowSaleItem = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowSaleItem = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowPurchaseItem")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowPurchaseItem = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowPurchaseItem = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowAdition2")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowAdition2 = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowAdition2 = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowAdition1")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowAdition1 = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowAdition1 = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowAdition1Detail")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowAdition1Detail = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowAdition1Detail = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ShowAdition2Detail")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            ShowAdition2Detail = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            ShowAdition2Detail = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "NotCheckStockMali")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotCheckStockMali = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotCheckStockMali = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Edititemname")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            Edititemname = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            Edititemname = false;
                        }
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "sematfinalQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematfinalQ = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematfinalQ = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematetebariQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematetebariQ = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematetebariQ = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DefualtTasfie")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            DefualtTasfie = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            DefualtTasfie = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "DefualtSalebuytype")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            DefualtSalebuytype = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            DefualtSalebuytype = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematnaghdiQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematnaghdiQ = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematnaghdiQ = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "MaliQ")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            MaliQ = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            MaliQ = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematbargiri")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematbargiri = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematbargiri = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematsendbar")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematsendbar = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematsendbar = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "cmdsematersalnashodebar")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematersalnashodebar = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematersalnashodebar = null;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematokbuy")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            cmdsematokbuy = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            cmdsematokbuy = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "reservesanad")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            reservesanad = Int32.Parse(row4["TNZ_sharh"].ToString());
                        }
                        else
                        {
                            reservesanad = -1;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematPayOrder")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            sematPayOrder = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            sematPayOrder = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Unit2")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            Unit2 = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            Unit2 = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Unit3")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            Unit3 = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            Unit3 = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "semat1reqleave")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            cmdsemat1reqleave = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            cmdsemat1reqleave = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "semat2reqleave")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            cmdsemat2reqleave = row4["TNZ_sharh"].ToString();
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "sematestelam")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            cmdsematestelam = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            cmdsematestelam = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "pishfarsgheymat")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            pishfarsgheymat = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            pishfarsgheymat = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SematCreate")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            SematCreate = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            SematCreate = "";
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SematPricing")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() != "")
                        {
                            SematPricing = row4["TNZ_sharh"].ToString();
                        }
                        else
                        {
                            SematPricing = "";
                        }
                    }
                    catch
                    {
                    }
                }
                //else if (row4["TNZ_name"].ToString() == "moendaryafti")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            moendaryafti = row4["TNZ_sharh"].ToString();
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                //else if (row4["TNZ_name"].ToString() == "moenvosol")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            moenvosol = row4["TNZ_sharh"].ToString();
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                //else if (row4["TNZ_name"].ToString() == "moensandogh")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            moensandogh = row4["TNZ_sharh"].ToString();
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                //else if (row4["TNZ_name"].ToString() == "moenvakhast")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            moenvakhast = row4["TNZ_sharh"].ToString();
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                //else if (row4["TNZ_name"].ToString() == "moentazmini")
                //{
                //    try
                //    {
                //        if (row4["TNZ_sharh"].ToString() != "")
                //        {
                //            moentazmini = row4["TNZ_sharh"].ToString();
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                else if (row4["TNZ_name"].ToString() == "txtapi")
                {
                    try
                    {
                        txtapi = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtrobat")
                {
                    try
                    {
                        txtrobat = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtnumbersms")
                {
                    try
                    {
                        txtnumbersms = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtnumbersms2")
                {
                    try
                    {
                        txtnumbersms2 = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtusersms")
                {
                    try
                    {
                        txtusersms = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WideirPass")
                {
                    try
                    {
                        WideirPass = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WideirUsername")
                {
                    try
                    {
                        WideirUsername = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "wideirphon1")
                {
                    try
                    {
                        wideirphon1 = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "wideirphon2")
                {
                    try
                    {
                        wideirphon2 = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WideEtebar")
                {
                    try
                    {
                        WideEtebar = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WideSendEtebarNumber")
                {
                    try
                    {
                        WideSendEtebarNumber = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "ApiTockenSmsir")
                {
                    try
                    {
                        ApiTockenSmsir = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "smsirphon2")
                {
                    try
                    {
                        smsirphon2 = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "smsirphon1")
                {
                    try
                    {
                        smsirphon1 = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "reportpoya")
                {
                    try
                    {
                        reportpoya = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "txtpasssms")
                {
                    try
                    {
                        txtpasssms = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtdomainsms")
                {
                    try
                    {
                        txtdomainsms = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtnameforoshgah")
                {
                    try
                    {
                        namecompany = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtaddgmail")
                {
                    try
                    {
                        txtaddgmail = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                //else if (row4["TNZ_name"].ToString() == "countcall")
                //{
                //    try
                //    {
                //        countcall = row4["TNZ_sharh"].ToString();
                //    }
                //    catch
                //    {
                //    }
                //}
                else if (row4["TNZ_name"].ToString() == "countcalldesc")
                {
                    try
                    {
                        countcalldesc = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "textpassgmail")
                {
                    try
                    {
                        txtpassgmail = row4["TNZ_sharh"].ToString();
                        // notifyIcon1.Text = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "txtcountroz")
                {
                    try
                    {
                        coundschedure = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "barbariname")
                {
                    try
                    {
                        barbariname = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "PrintSystem")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            PrintSystem = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            PrintSystem = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "Watermark")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            Watermark = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            Watermark = false;
                        }
                    }
                    catch
                    {
                    }

                }

                else if (row4["TNZ_name"].ToString() == "checkbargiriranande")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            checkbargiriranande = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            checkbargiriranande = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "TransferCallNextDay")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TransferCallNextDay = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TransferCallNextDay = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "SaleExport")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SaleExport = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SaleExport = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "showcolumn")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            showcolumn = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            showcolumn = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "NotShowWhatsup")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            NotShowWhatsup = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            NotShowWhatsup = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "reportlocal")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            reportlocal = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            reportlocal = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "chkdesccall")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            chkdesccall = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            chkdesccall = false;
                        }
                    }
                    catch
                    {
                    }

                }
                else if (row4["TNZ_name"].ToString() == "Notshowtell")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            Notshowtell = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            Notshowtell = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TextFileS")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TextFileS = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TextFileS = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TextFileD")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TextFileD = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TextFileD = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "TextFileV")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            TextFileV = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            TextFileV = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SeparateSmsCustomer")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SeparateSmsCustomer = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SeparateSmsCustomer = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "customerstouser")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            customerstouser = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            customerstouser = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "AssigncustomerTouser")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            AssigncustomerTouser = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            AssigncustomerTouser = false;
                        }
                    }
                    catch
                    {
                    }
                }
                //sepidar
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Access")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SepidarApi_Access = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SepidarApi_Access = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_SendRecieved")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SepidarApi_SendRecieved = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SepidarApi_SendRecieved = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Serial_edited")
                {
                    try
                    {
                        if (row4["TNZ_sharh"].ToString() == "1")
                        {
                            SepidarApi_Serial_edited = true;
                        }
                        if (row4["TNZ_sharh"].ToString() == "0")
                        {
                            SepidarApi_Serial_edited = false;
                        }
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Version")
                {
                    try
                    {
                        SepidarApi_Version = row4["TNZ_sharh"].ToInt();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Serial")
                {
                    try
                    {
                        SepidarApi_Serial = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_UrlDomain")
                {
                    try
                    {
                        SepidarApi_UrlDomain = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Port")
                {
                    try
                    {
                        SepidarApi_Port = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Username")
                {
                    try
                    {
                        SepidarApi_Username = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Password")
                {
                    try
                    {
                        SepidarApi_Password = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_CreateFactor")
                {
                    try
                    {
                        SepidarApi_CreateFactor = row4["TNZ_sharh"].ToInt();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_Cypher")
                {
                    try
                    {
                        SepidarApi_Cypher = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_IV")
                {
                    try
                    {
                        SepidarApi_IV = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_publickey")
                {
                    try
                    {
                        SepidarApi_publickey = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SepidarApi_DeviceName")
                {
                    try
                    {
                        SepidarApi_DeviceName = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                //else if (row4["TNZ_name"].ToString() == "PmAutomaticBargiriCar")
                //{
                //    try
                //    {
                //        PmAutomaticBargiriCar = Convert.ToBoolean(row4["TNZ_sharh"].ToString());
                //    }
                //    catch
                //    {
                //        PmAutomaticBargiriCar = false;
                //    }
                //}
                else if (row4["TNZ_name"].ToString() == "BackupDesctination")
                {
                    try
                    {
                        BackupDesctination = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "fontrow")
                {
                    try
                    {
                        fontrowT = row4["TNZ_sharh"].ToString();
                        string[] str = row4["TNZ_sharh"].ToString().Split('=');
                        if (str[1].ToString() == "Bold")
                        {
                            fontstyle = FontStyle.Bold;
                        }
                        if (str[1].ToString() == "Italic")
                        {
                            fontstyle = FontStyle.Italic;
                        }
                        if (str[1].ToString() == "Regular")
                        {
                            fontstyle = FontStyle.Regular;
                        }
                        if (str[1].ToString() == "Strikeout")
                        {
                            fontstyle = FontStyle.Strikeout;
                        }
                        if (str[1].ToString() == "Underline")
                        {
                            fontstyle = FontStyle.Underline;
                        }
                        string[] str1 = row4["TNZ_sharh"].ToString().Split(';');
                        string[] str2 = str1[1].Split('p');
                        float flt = float.Parse(str2[0].ToString());
                        fontrow = new System.Drawing.Font(str1[0], flt, fontstyle);
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "fontwatermark")
                {
                    try
                    {
                        fontwatermarkT = row4["TNZ_sharh"].ToString();
                        string[] str = row4["TNZ_sharh"].ToString().Split('=');
                        if (str[1].ToString() == "Bold")
                        {
                            fontstyle = FontStyle.Bold;
                        }
                        if (str[1].ToString() == "Italic")
                        {
                            fontstyle = FontStyle.Italic;
                        }
                        if (str[1].ToString() == "Regular")
                        {
                            fontstyle = FontStyle.Regular;
                        }
                        if (str[1].ToString() == "Strikeout")
                        {
                            fontstyle = FontStyle.Strikeout;
                        }
                        if (str[1].ToString() == "Underline")
                        {
                            fontstyle = FontStyle.Underline;
                        }
                        string[] str1 = row4["TNZ_sharh"].ToString().Split(';');
                        string[] str2 = str1[1].Split('p');
                        float flt = float.Parse(str2[0].ToString());
                        fontwatermark = new System.Drawing.Font(str1[0], flt, fontstyle);
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "PAnelSms")
                {
                    try
                    {
                        PAnelSms = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Typeprint")
                {
                    try
                    {
                        Typeprint = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "printtamin")
                {
                    try
                    {
                        printtamin = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Sizepaper")
                {
                    try
                    {
                        Papersize = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SanatCustomer")
                {
                    try
                    {
                        SanatCustomer = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SearchType")
                {
                    try
                    {
                        SearchType = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "SendQuentionMali")
                {
                    try
                    {
                        SendQuentionMali = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "CycleForm")
                {
                    try
                    {
                        CycleForm = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "StateConnectTomali")
                {
                    try
                    {
                        StateConnectTomali = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "Sendtomali")
                {
                    try
                    {
                        Sendtomali = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "karshenasforosh")
                {
                    try
                    {
                        karshenasforosh = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "maghtabargiri")
                {
                    try
                    {
                        maghtabargiri = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "CheckEnhesar")
                {
                    try
                    {
                        CheckEnhesar = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "formatwhatsup")
                {
                    try
                    {
                        formatwhatsup = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "blacklist")
                {
                    try
                    {
                        blacklist = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "CheckLRFM")
                {
                    try
                    {
                        CheckLRFM = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "WhatsupTamin")
                {
                    try
                    {
                        WhatsupTamin = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }

                else if (row4["TNZ_name"].ToString() == "textwatermark")
                {
                    try
                    {
                        textwatermark = row4["TNZ_sharh"].ToString();
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colordatagrid")
                {
                    try
                    {
                        colordatagrid = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colorcustomized")
                {
                    try
                    {
                        colorcustomized = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colorRangL")
                {
                    try
                    {
                        colorRangL = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colorRangR")
                {
                    try
                    {
                        colorRangR = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colorRangF")
                {
                    try
                    {
                        colorRangF = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colorRangM")
                {
                    try
                    {
                        colorRangM = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colordsablerow")
                {
                    try
                    {
                        colordsablerow = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
                else if (row4["TNZ_name"].ToString() == "colordsablerowfor")
                {
                    try
                    {
                        colordsablerowfor = Int32.Parse(row4["TNZ_sharh"].ToString());
                    }
                    catch
                    {
                    }
                }
            }
            location_report = Path.GetDirectoryName(Application.ExecutablePath); // خواندن مسیر سرور
            if (!reportlocalUser)
            {
                if (!FrmMain.reportlocal)
                {
                    string[] str;
                    str = FrmConnect.servername.Split('\\');
                    location_report = "\\\\" + str[0];
                    // MessageBox.Show("a2 "+ strreport);
                }
            }

        }*/
        }
    }
}
