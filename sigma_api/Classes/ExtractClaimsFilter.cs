using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class ExtractClaimsFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            // نمونه: گرفتن یوزرنیم از کلیم
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var FullName = user.FindFirst(ClaimTypes.Name)?.Value;
            var token = user.FindFirst(ClaimTypes.Authentication)?.Value;
            var UserType = user.FindFirst(ClaimTypes.Role)?.Value;
            var ChatId = user.FindFirst(ClaimTypes.Email)?.Value;

            // می‌تونی این دیتاها رو توی HttpContext.Items بذاری که بعداً تو کنترلر بخونی
            context.HttpContext.Items["UserId"] = userId;
            context.HttpContext.Items["FullName"] = FullName;
            context.HttpContext.Items["token"] = token;
            context.HttpContext.Items["UserType"] = UserType;
            context.HttpContext.Items["ChatId"] = ChatId;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // بعد از اجرای اکشن (اختیاری)
    }
}