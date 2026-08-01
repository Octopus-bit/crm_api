using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sigma_api.Classes
{
    public static class cls_convert
    {
        public static int ToInt(this object value)
        {
            int i = 0;
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    i = Convert.ToInt32(value);
                }
            }
            catch
            {
            }
            return i;
        }


        public static decimal ToDecimal(this object value)
        {
            decimal i = 0;
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    i = Convert.ToDecimal(value);
                }
            }
            catch
            {
            }
            return i;
        }

        public static double ToDouble(this object value)
        {
            double i = 0;
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    i = Convert.ToDouble(value);
                }
            }
            catch
            {
            }
            return i;
        }

        public static long ToMab(this object value)
        {
            long i = 0;
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    i = Convert.ToInt64(value);
                }
            }
            catch
            {
            }
            return i;
        }


        public static bool ToBoolean(this object value)
        {
            if (value != null && value != DBNull.Value && value.ToString().IsNumeric())
                value = value.ToInt();
            bool tf = false;
            try
            {
                if (value != null && value != DBNull.Value)
                {
                    tf = Convert.ToBoolean(value);
                }
            }
            catch
            {
            }
            return tf;
        }


        public static string ToMoney(this object value)
        {
            try
            {
                return string.Format("{0:N0}", value.ToMab());
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static bool IsNumeric(this object value)
        {
            try
            {
                Convert.ToInt64(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string TryString(this object value)
        {
            try
            {
                if (value != null)
                {
                    if (value.ToString().Length > 0 && string.IsNullOrEmpty(value.ToString()) == false)
                    {
                        return value.ToString();
                    }
                    else
                        return null;

                }
                else return null;

            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool IsNull(this object value)
        {
            try
            {
                return string.IsNullOrEmpty(value.ToString());
            }
            catch
            {
                return false;
            }
        }
    }
}
