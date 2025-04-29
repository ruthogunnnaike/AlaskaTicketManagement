
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AlaskaTicketManagement.Authentication
{
    public class ApiAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock timeProvider)
            : base(options, logger, encoder, timeProvider)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Create a fake identity
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "DummyUser"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("EventManager", "true")
        };
            var identity = new ClaimsIdentity(claims, "DummyAuthentication");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "DummyAuthentication");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

    }
}
