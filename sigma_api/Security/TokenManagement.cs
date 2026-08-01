using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Concurrent;
using sigma_api.Classes;

namespace sigma_api.Security
{
    public class TokenManagement // کلاس احراز توکن
    {
        public static bool IsCreated = false;
        private static readonly ConcurrentDictionary<string, (DataTable Data, DateTime Expires)> _tokenCache
            = new ConcurrentDictionary<string, (DataTable, DateTime)>(StringComparer.Ordinal);
        private static readonly TimeSpan TokenCacheDuration = TimeSpan.FromMinutes(5);
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
            if (!string.IsNullOrEmpty(token))
            {
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length);
                }

                if (_tokenCache.TryGetValue(token, out var cached) && cached.Expires > DateTime.UtcNow)
                    return cached.Data;

                cls_public cp = new cls_public();

                SqlParameter[] parameters =
                {
                    new SqlParameter("token",token),
                };
                var result = cp.select_row("API_GetUserByToken", parameters);
                if (result != null && result.Rows.Count > 0)
                {
                    _tokenCache[token] = (result, DateTime.UtcNow.Add(TokenCacheDuration));
                }
                return result;
            }
            return null;
        }

        public static void InvalidateToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
                _tokenCache.TryRemove(token, out _);
        }
    }
}
