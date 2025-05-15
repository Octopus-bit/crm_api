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


    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Token"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Token Header"));
        }

        var token = Request.Headers["Token"].ToString();

        if (!Guid.TryParse(token, out var guid))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Token"));
        }

        // اینجا می‌تونی مثلا چک کنی که GUID تو دیتابیس معتبر باشه یا نه

        DataTable dt_UserInfo = TokenManagement.ReadToken(token.ToString());

        if (dt_UserInfo.Rows.Count <= 0)
        {
            return Task.FromResult(AuthenticateResult.Fail("Failure to authenticate"));
        }


        //User-Info
        int UserType = dt_UserInfo.Rows[0]["UserType"].ToInt();
        int UserId = dt_UserInfo.Rows[0]["UserId"].ToInt();
        string FullName = dt_UserInfo.Rows[0]["FullName"].ToString();

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
                return Task.FromResult(AuthenticateResult.Fail("Failure to authenticate"));
            }
            else if (controllerName == "android" && UserType == 1)
            {
                return Task.FromResult(AuthenticateResult.Fail("Failure to authenticate"));
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
}