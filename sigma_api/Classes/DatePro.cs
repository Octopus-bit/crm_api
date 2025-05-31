using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace sigma_api.Classes
{
    public partial class DatePro
    {

        public string y, m, d;
        string years1;
        PersianCalendar data = new PersianCalendar();
        DateTime dt = DateTime.Today;



        public string Date;

        public string GetDateEnglish(string DatePertion)
        {
            string Y, M, D;
            DateTime dt = new DateTime(Convert.ToInt32(DatePertion.Split('/')[0]), Convert.ToInt32(DatePertion.Split('/')[1]), Convert.ToInt32(DatePertion.Split('/')[2]), data);
            string checkDate = dt.ToString().Substring(0, 10).TrimEnd(' ');
            string[] splitdatre = checkDate.Split('/');
            Y = splitdatre[2];
            M = splitdatre[0];
            D = splitdatre[1];
            if (M.Length == 1)
            {
                M = "0" + M;
            }
            if (D.Length == 1)
            {
                D = "0" + D;
            }
            Date = M + "/" + D + "/" + Y;
            return Date;

        }

        public string DateSumDay(string DatePertion, int sum)
        {
            string Y, M, D;
            Y = DatePertion.Split('/')[0];
            M = DatePertion.Split('/')[1];
            D = DatePertion.Split('/')[2];
            int day = int.Parse(D);
            for (int i = 0; i < sum; i++)
            {
                if (int.Parse(M) > 6)
                {
                    day++;
                    if (day >= 30)
                    {
                        day = 0;
                        M = (int.Parse(M) + 1).ToString();
                        if (int.Parse(M) > 12)
                        {
                            M = 1.ToString();
                            Y = (int.Parse(Y) + 1).ToString();
                        }
                    }
                }
                else
                {
                    day++;
                    if (day >= 31)
                    {
                        day = 0;
                        M = (int.Parse(M) + 1).ToString();
                        if (int.Parse(M) > 12)
                        {
                            M = 1.ToString();
                            Y = (int.Parse(Y) + 1).ToString();
                        }
                    }
                }
            }
            D = day.ToString();
            if (M.Length == 1)
            {
                M = "0" + M;
            }
            if (D.Length == 1)
            {
                D = "0" + D;
            }
            Date = Y + "/" + M + "/" + D;
            return Date;

        }

        public string GetDatePertion(string DateEnglish)
        {

            string Y, M, D;
            string[] Data = DateEnglish.Split('/');
            M = Data[0];
            D = Data[1];
            Y = Data[2].Substring(0, 4);
            if (M.Length == 1)
            {
                M = "0" + M;
            }
            if (D.Length == 1)
            {
                D = "0" + D;
            }
            DateTime dt = new DateTime(int.Parse(Y), int.Parse(M), int.Parse(D));

            Y = data.GetYear(dt).ToString();
            M = data.GetMonth(dt).ToString();
            D = data.GetDayOfMonth(dt).ToString();
            if (M.Length == 1)
            {
                M = "0" + M;
            }
            if (D.Length == 1)
            {
                D = "0" + D;
            }
            Date = Y + "/" + M + "/" + D;
            return Date;
        }


        public string[] GetNextDay(string Date, int Days)
        {
            string[] Data = new string[Days];
            int d = int.Parse(Date.Split('/')[2]);
            int m = int.Parse(Date.Split('/')[1]);
            int y = int.Parse(Date.Split('/')[0]);
            int daysInMonth = data.GetDaysInMonth(y, m);
            for (int i = 0; i < Days; i++)
            {
                if (d > daysInMonth)
                {
                    m++;
                    d = 1;
                }
                if (m > 12)
                {
                    y++;
                    m = 1;
                }

                string M, D;
                M = m.ToString();
                D = d++.ToString();
                if (M.Length == 1)
                {
                    M = "0" + M;
                }
                if (D.Length == 1)
                {
                    D = "0" + D;
                }
                Data[i] = y + "/" + M + "/" + D;
            }

            return Data;
        }





        public string GetDataNow(int NextYear = 0, int NextMonth = 0)
        {
            DateTime time = DateTime.Now;
            DateTime tarikh = DateTime.Today;

            string y1 = data.GetYear(tarikh).ToString();
            string m1 = data.GetMonth(tarikh).ToString();
            string d1 = data.GetDayOfMonth(tarikh).ToString();
            y1 = (int.Parse(y1) + NextYear).ToString();
            m1 = (int.Parse(m1) + NextMonth).ToString();
            string y = y1;
            if (m1.Length < 2)
            {
                m = "0" + m1;
            }
            else
            {
                m = m1;
            }

            if (d1.Length < 2)
            {
                d = "0" + d1;
            }
            else
            {
                d = d1;
            }

            return y + "/" + m + "/" + d;
        }

        public string GetDataNow(string DatePertion, int NextYear = 0, int NextMonth = 0)
        {

            string y1 = DatePertion.Split('/')[0].ToString();
            string m1 = DatePertion.Split('/')[1].ToString();
            string d1 = DatePertion.Split('/')[2].ToString();
            y1 = (int.Parse(y1) + NextYear).ToString();
            m1 = (int.Parse(m1) + NextMonth).ToString();
            string y = y1;
            if (m1.Length < 2)
            {
                m = "0" + m1;
            }
            else
            {
                m = m1;
            }

            if (d1.Length < 2)
            {
                d = "0" + d1;
            }
            else
            {
                d = d1;
            }

            return y + "/" + m + "/" + d;
        }





    }
}
