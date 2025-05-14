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
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // می‌تونی این دیتاها رو توی HttpContext.Items بذاری که بعداً تو کنترلر بخونی
            context.HttpContext.Items["Username"] = username;
            context.HttpContext.Items["UserId"] = userId;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // بعد از اجرای اکشن (اختیاری)
    }
}