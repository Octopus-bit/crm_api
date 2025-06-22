using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using sigma_api.Classes;

namespace sigma_api.Security
{
    public class TokenManagement // کلاس احراز توکن
    {
        public static bool IsCreated = false;
        public static string WriteToken(int UserId, int TypeToken) //این تابع توکن جدیدی برای کاربر ایجاد میکند
        {
            string token = null;
            cls_public cp = new cls_public();
            int retry = 1;
            while (retry <= 3)
            {
                token = Guid.NewGuid().ToString();
                SqlParameter[] parameters2 =
                {
                    new SqlParameter("user_id", UserId),
                    new SqlParameter("token", coding.SigmaEncryptString(token)),
                    new SqlParameter("typeToken", TypeToken),
                };
                if (cp.update("API_SetTokenUsers", parameters2))
                {
                    IsCreated = true;
                    break;
                }
                else
                {
                    token = null;
                }
                retry++;
            }

            return coding.SigmaEncryptString(token);
        }

        public static DataTable ReadToken(string token) // تابع خواندن توکن از دیتابیس و دریافت اطلاعات یوزر مربوط به توکن
        {
            cls_public cp = new cls_public();

            if (!string.IsNullOrEmpty(token))
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length);
                }

                SqlParameter[] parameters =
                {
                    new SqlParameter("token",token),
                };
                return cp.select_row("API_GetUserByToken", parameters);
            }
            return null;
        }
    }
}
