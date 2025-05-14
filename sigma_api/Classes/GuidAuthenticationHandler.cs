using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
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

        int UserId = TokenManagement.ReadToken(token.ToString());

        if (UserId <= 0 || string.IsNullOrEmpty(UserId.ToString()))
            return Task.FromResult(AuthenticateResult.Fail("Failure to authenticate"));


        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),  // این کلایم برای دسترسی به آی‌دی
            new Claim(ClaimTypes.Name, "TokenUser")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));

    }
}