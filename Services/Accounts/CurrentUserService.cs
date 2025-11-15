namespace ExaminationSystem.Services.Accounts
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.JsonWebTokens;
    using System.Security.Claims;

    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string? UserId => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }

}
