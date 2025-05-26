using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace sigma_api.Classes
{
    class ShamsiDate
    {
        public ShamsiDate()
        {
        }

        
        void sample()
        {
            //حذف جند رکورد  gridex
            //GridEXRow[] checkedRows;
            //checkedRows = GridEX1.GetCheckedRows();
            //if (checkedRows.Length == 0)
            //{
            //    MessageBox.Show("Select at least 1 message to be deleted.");
            //}
            //else
            //{
            //    string message;
            //    message = String.Format("You are about to delete {0} message(s)." +
            //                "\n\rDo you want to continue?", checkedRows.Length);
            //    if (MessageBox.Show(message, "Janus Tutorial", MessageBoxButtons.YesNo) ==
            //        System.Windows.Forms.DialogResult.Yes)
            //    {
            //        foreach (GridEXRow row in checkedRows)
            //        {
            //            ((DataRowView)row.DataRow).Delete();
            //        }
            //    }

            //}
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
            //حذف یک رکورد
            //GridEXRow item = gridEX1.CurrentRow;
            //if (item != null && item.RowType == RowType.Record)
            //{
            //    //Delete the row.
            //    item.Delete();
            //}
            //else
            //{
            //    MessageBox.Show("Select an item to delete.");
            //}
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
            //حذف کل رکورد 
            //this.gridEX1.ClearItems();
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////
        }
        public enum TDateKind
        {
            dkSolar = 0,
            dkGregorian = 1
        }
        static byte[] LeapMonth = { 12 /*Esfand*/, 2 /*February*/};
        static byte[,] DaysOfMonths = new byte[,]{
                                    {  31,  31,  31,  31,  31,  31,  30,  30,  30,  30,  30,  29 }
                                                     /* Far, Ord, Kho, Tir, Mor, Sha, Meh, Aba, Aza, Day, Bah,^Esf */,
                                    {  31,  28,  31,  30,  31,  30,  31,  31,  30,  31,  30,  31 }
                                                     /* Jan,^Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec */
                                                 };
        static uint[,] DaysToMonth = new uint[,]{
                                       {   0,  31,  62,  93, 124, 155, 186, 216, 246, 276, 306, 336, 365 }
                                                    /* Far, Ord, Kho, Tir, Mor, Sha, Meh, Aba, Aza, Day, Bah,^Esf, *** */,
                                       {   0,  31,  59,  90, 120, 151, 181, 212, 243, 273, 304, 334, 365 }
                                                    /* Jan,^Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec, *** */
                                                };

        [System.Diagnostics.DebuggerStepThrough()]
        static bool IsLeapYear(TDateKind DateKind, uint Year)
        {
            bool Result = false;
            if (DateKind == TDateKind.dkSolar)
                Result = ((((Year + 38) * 31) % 128) <= 30);
            else
                Result = ((Year % 4) == 0) && (((Year % 100) != 0) || ((Year % 400) == 0));
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static uint DaysOfMonth(TDateKind DateKind, uint Year, uint Month)
        {
            uint Result = 0;
            if ((Year != 0) && (Month > 0) && (Month < 13))
            {
                Result = DaysOfMonths[(int)DateKind, (int)Month - 1];
                if ((Month == LeapMonth[(int)DateKind]) && IsLeapYear(DateKind, Year))
                    Result++;
            }
            else
                Result = 0;
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static bool IsDateValid(TDateKind DateKind, uint Year, uint Month, uint Day)
        {
            bool Result;
            Result = (Year != 0) && (Month >= 1) && (Month <= 12) &&
                (Day >= 1) && (Day <= DaysOfMonth(DateKind, Year, Month));
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static uint DaysToDate(TDateKind DateKind, uint Year, uint Month, uint Day)
        {
            uint Result;
            if (IsDateValid(DateKind, Year, Month, Day))
            {
                Result = DaysToMonth[(int)DateKind, Month - 1] + Day;
                if ((Month > LeapMonth[(int)DateKind]) && IsLeapYear(DateKind, Year))
                    Result++;
            }
            else
                Result = 0;
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static bool DateOfDay(TDateKind DateKind, uint Days, uint Year, ref uint Month, ref uint Day)
        {
            uint LeapDay = 0;
            bool Result = false;
            Month = 0;
            Day = 0;
            for (int m = 2; m < 14; m++)
            {
                if ((m > LeapMonth[(int)DateKind]) && IsLeapYear(DateKind, Year))
                    LeapDay = 1;
                if (Days <= (DaysToMonth[(int)DateKind, m - 1] + LeapDay))
                {
                    Month = (uint)m - 1;
                    if (Month <= LeapMonth[(int)DateKind])
                        LeapDay = 0;
                    Day = Days - (DaysToMonth[(int)DateKind, (int)Month - 1] + LeapDay);
                    break;
                }
            }
            Result = IsDateValid(DateKind, Year, Month, Day);
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static bool GregorianToSolar(ref uint Year, ref uint Month, ref uint Day)
        {
            int mabna_sal = 1403;
            uint LeapDay, Days;
            bool PrevGregorianLeap;
            bool Result;
            if (IsDateValid(TDateKind.dkGregorian, Year, Month, Day))
            {
                PrevGregorianLeap = IsLeapYear(TDateKind.dkGregorian, Year - 1);
                Days = DaysToDate(TDateKind.dkGregorian, Year, Month, Day);
                Year -= 622;
                if (IsLeapYear(TDateKind.dkSolar, Year))
                    LeapDay = 1;
                else
                    LeapDay = 0;
                int midd_year = Convert.ToInt32(Year) - mabna_sal;
                midd_year %= 4;
                if (midd_year == 0)
                    LeapDay = 1;
                else
                    LeapDay = 0;
                if (PrevGregorianLeap && (LeapDay == 1))
                    Days += 287;
                else
                    Days += 286;
                if (Days > (365 + LeapDay))
                {
                    Year++;
                    Days -= (365 + LeapDay);
                }


                Result = DateOfDay(TDateKind.dkSolar, Days, Year, ref Month, ref Day);
            }
            else
                Result = false;
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static bool SolarToGregorian(ref uint Year, ref uint Month, ref uint Day)
        {
            uint LeapDay, Days;
            bool PrevSolarLeap;
            bool Result;
            if (IsDateValid(TDateKind.dkSolar, Year, Month, Day))
            {
                PrevSolarLeap = IsLeapYear(TDateKind.dkSolar, Year - 1);
                Days = DaysToDate(TDateKind.dkSolar, Year, Month, Day);
                Year += 621;
                if (IsLeapYear(TDateKind.dkGregorian, Year))
                    LeapDay = 1;
                else
                    LeapDay = 0;
                if (PrevSolarLeap && (LeapDay == 1))
                    Days += 80;
                else
                    Days += 79;
                if (Days > (365 + LeapDay))
                {
                    Year++;
                    Days -= (365 + LeapDay);
                }
                Result = DateOfDay(TDateKind.dkGregorian, Days, Year, ref Month, ref Day);
            }
            else
                Result = false;
            return Result;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string NowYear()
        {
            cls_public cp = new cls_public();
            System.DateTime dt;
            uint y, m, d;
            dt = cp.GetDateTime();
            y = (uint)dt.Year;
            m = (uint)dt.Month;
            d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            return y.ToString();
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string NowMounth()
        {
            cls_public cp = new cls_public();
            System.DateTime dt;
            uint y, m, d;
            dt = cp.GetDateTime();
            y = (uint)dt.Year;
            m = (uint)dt.Month;
            d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            string s = m.ToString();
            if (s.Length == 1)
                s = "0" + s;
            return s;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string NowDay()
        {
            cls_public cp = new cls_public();
            System.DateTime dt;
            uint y, m, d;
            dt = cp.GetDateTime();
            y = (uint)dt.Year;
            m = (uint)dt.Month;
            d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            string s = d.ToString();
            if (s.Length == 1)
                s = "0" + s;
            return s;
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public bool IsForMatDate(ref string Input)
        {
            int temp = 0;
            string s = Input;
            s = s.Replace(" ", "");
            string[] ss = s.Split('/');
            if (ss.Length != 3)
                return false;
            if (!(ss[0].Length != 2) && !(ss[0].Length != 4))
                return false;
            if (ss[0].Length == 2)
            {
                try
                {
                    temp = int.Parse(ss[0]);
                }
                catch
                {
                    return false;
                }
                if (temp < 60)
                    return false;
                ss[0] = ShamsiDate.NowYear().Substring(0, 2) + ss[0];
            }
            else
            {
                try
                {
                    temp = int.Parse(ss[0]);
                }
                catch
                {
                    return false;
                }
                if (temp < 1360 || temp > 1400)
                    return false;
            }
            //checking Month
            if (ss[1].Length > 2 || ss[1].Length < 1)
                return false;
            try
            {
                temp = int.Parse(ss[1]);
            }
            catch
            {
                return false;
            }
            if (temp > 12 || temp < 1)
                return false;
            if (ss[1].Length == 1)
                ss[1] = "0" + ss[1];
            //checking Month 
            if (ss[2].Length > 2 || ss[2].Length < 1)
                return false;
            try
            {
                temp = int.Parse(ss[2]);
            }
            catch
            {
                return false;
            }
            if (temp > 31 || temp < 1)
                return false;
            if (ss[2].Length == 1)
                ss[2] = "0" + ss[2];
            Input = ss[0] + "/" + ss[1] + "/" + ss[2];
            return true;


        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string FormatDate(int year, int month, int day)
        {
            string y, m, d;
            y = year.ToString();
            m = month.ToString();
            d = day.ToString();
            if (m.Length == 1) m = "0" + m;
            if (d.Length == 1) d = "0" + d;
            return (y + "/" + m + "/" + d);
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string FormatDateSlashless(int year, int month, int day)
        {
            string y, m, d;
            y = year.ToString();
            m = month.ToString();
            d = day.ToString();
            if (m.Length == 1) m = "0" + m;
            if (d.Length == 1) d = "0" + d;
            return (y + m + d);
        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string NowDate()
        {
            return ShamsiDate.NowYear() + "/" + ShamsiDate.NowMounth() + "/" + ShamsiDate.NowDay();

        }

        [System.Diagnostics.DebuggerStepThrough()]
        static public string MiladiToShamsi(DateTime InDate)
        {
            return MiladiToShamsi(InDate.Year, InDate.Month, InDate.Day);
        }
        static public string MiladiToShamsiWithinZero(string InDate)
        {
            string[] str = InDate.Split('/');
            if (str[1].StartsWith("0"))
                str[1] = str[1].Substring(1, 1);
            if (str[2].StartsWith("0"))
                str[2] = str[2].Substring(1, 1);
            return str[0] + '/' + str[1] + '/' + str[2];
        }
        [System.Diagnostics.DebuggerStepThrough()]
        static public string MiladiToShamsi(int year, int month, int day)
        {
            uint y = (uint)year, m = (uint)month, d = (uint)day;
            GregorianToSolar(ref y, ref m, ref d);
            return (FormatDate((int)y, (int)m, (int)d));
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static string GetMonthName(int Month)
        {
            string[] Months = {"فرودين"
                                  ,"ارديبهشت"
                                  ,"خرداد"
                                  ,"تير"
                                  ,"مرداد"
                                  ,"شهريور"
                                  ,"مهر"
                                  ,"آبان"
                                  ,"آذر"
                                  ,"دي"
                                  ,"بهمن"
                                  ,"اسفند"};
            return Months[Month - 1];
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static int ShamsiDayOf(DateTime dt)
        {
            uint y = (uint)dt.Year, m = (uint)dt.Month, d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            return (int)d;

        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static int ShamsiMonthOf(DateTime dt)
        {
            uint y = (uint)dt.Year, m = (uint)dt.Month, d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            return (int)m;

        }

        [System.Diagnostics.DebuggerStepThrough()]
        public static int ShamsiYearOf(DateTime dt)
        {
            uint y = (uint)dt.Year, m = (uint)dt.Month, d = (uint)dt.Day;
            GregorianToSolar(ref y, ref m, ref d);
            return (int)y;

        }

        public static string systemDayOfWeek()
        {
            cls_public cp = new cls_public();
            string res = "";
            switch (cp.GetDateTime().DayOfWeek.ToString())
            {
                case "Saturday":
                    res = "شنبه";
                    break;

                case "Sunday":
                    res = "يكشنبه";
                    break;

                case "Monday":
                    res = "دوشنبه";
                    break;

                case "Tuesday":
                    res = "سه شنبه";
                    break;

                case "Wednesday":
                    res = "چهار شنبه";
                    break;

                case "Thursday":
                    res = "پنج شنبه";
                    break;

                case "Friday":
                    res = "جمعه";
                    break;
            }

            return res;

        }
        public static string systemDayOfWeekDate(DateTime dt)
        {
            string res = "";
            switch (dt.DayOfWeek.ToString())
            {
                case "Saturday":
                    res = "شنبه";
                    break;

                case "Sunday":
                    res = "يكشنبه";
                    break;

                case "Monday":
                    res = "دوشنبه";
                    break;

                case "Tuesday":
                    res = "سه شنبه";
                    break;

                case "Wednesday":
                    res = "چهار شنبه";
                    break;

                case "Thursday":
                    res = "پنج شنبه";
                    break;

                case "Friday":
                    res = "جمعه";
                    break;
            }

            return res;

        }
        static public string ChangeDateWithinZero(string InDate)
        {
            string[] str = InDate.Split('/');
            if (str[1].Length == 1)
                str[1] = "0" + str[1];
            if (str[2].Length == 1)
                str[2] = "0" + str[2];
            return str[0] + '/' + str[1] + '/' + str[2];
        }
        public static DateTime Shamsi2Miladi(string _date)
        {
            PersianCalendar p = new PersianCalendar();
            System.Globalization.PersianCalendar pc = new PersianCalendar();
            DateTime date = DateTime.Now;// classes.clsDB.GetDateTime();
            if (_date.Length == 8)
            {
                int year = int.Parse(_date.Substring(0, 2));
                int month = int.Parse(_date.Substring(3, 2));
                int day = int.Parse(_date.Substring(6, 2));

                int year1 = pc.ToFourDigitYear(year);
                int y1 = int.Parse(year1.ToString().Substring(0, 2));
                y1 *= 100;
                date = p.ToDateTime(y1 + year, month, day, 0, 0, 0, 0);
            }
            else
            {
                if (_date.Length == 10)
                {
                    int year = int.Parse(_date.Substring(0, 4));
                    int month = int.Parse(_date.Substring(5, 2));
                    int day = int.Parse(_date.Substring(8, 2));
                    date = p.ToDateTime(year, month, day, 0, 0, 0, 0);
                }
            }
            return date;
        }
    }
}
