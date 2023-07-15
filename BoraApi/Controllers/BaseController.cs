using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Bora.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        public string? AuthenticatedUserEmail
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    throw new AuthenticationException("Usuário não autenticado.");
                }
                var email = this.User.FindFirst(ClaimTypes.Email)?.Value;
                return email;
            }
        }
    }
}
