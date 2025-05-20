using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using sigma_api;
using sigma_api.Classes;
using sigma_api.ModelView;
using sigma_api.Security;

public class GuidAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public GuidAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
    )
        : base(options, logger, encoder, clock)
    {
    }

    private string _errorJson = null!;
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Token"))
        {
            _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "Missing Token Header");
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }

        var token = Request.Headers["Token"].ToString();

        if (!Guid.TryParse(token, out var guid))
        {
            _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "Invalid Token");
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }

        DataTable dt_UserInfo = new DataTable();
        // اینجا می‌تونی مثلا چک کنی که GUID تو دیتابیس معتبر باشه یا نه
        try
        {
            dt_UserInfo = TokenManagement.ReadToken(token.ToString());
        }
        catch
        { }
        if (cls_public.sqlError.success == false)
        {
            _errorJson = messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(),
                cls_public.sqlError.message);
            return Task.FromResult(AuthenticateResult.Fail("DBO_Unavailable"));
        }
        if (dt_UserInfo.Rows.Count <= 0)
        {
            _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "Failure to authenticate");
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }


        //User-Info
        int UserType = dt_UserInfo.Rows[0]["UserType"].ToInt();
        int UserId = dt_UserInfo.Rows[0]["UserId"].ToInt();
        string FullName = dt_UserInfo.Rows[0]["FullName"].ToString();

        if (UserId == 0)
        {
            _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "Failure to authenticate");
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }

        var endpoint = Context.GetEndpoint();
        if (endpoint != null)
        {
            var routePattern = (endpoint as RouteEndpoint)?.RoutePattern?.RawText;

            var controllerName = endpoint.Metadata
                .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ControllerName;

            var actionName = endpoint.Metadata
                .GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.ActionName;


            if (controllerName == "Crm" && UserType == 0)
            {
                _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(),
                    "Failure to authenticate");
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }
            else if (controllerName == "android" && UserType == 1)
            {
                _errorJson = messagebase.messages(false, ErrorCode.Authentication.GetHashCode(),
                    "Failure to authenticate");
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
            }
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),  // این کلایم برای دسترسی به آی‌دی
            new Claim(ClaimTypes.Name, FullName),
            new Claim(ClaimTypes.Authentication, token),
            new Claim(ClaimTypes.Role, UserType.ToString()),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));

    }
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Context.Response.StatusCode = 200; // چون می‌خوایم JSON برگردونیم، نه 401
        //Context.Response.ContentType = "application/json";
        Context.Response.ContentType = "text/plain";

        var responseJson = string.IsNullOrWhiteSpace(_errorJson)
            ? messagebase.messages(false, ErrorCode.Authentication.GetHashCode(), "Failure to authenticate")
            : _errorJson;

        return Context.Response.WriteAsync(responseJson);
    }
}