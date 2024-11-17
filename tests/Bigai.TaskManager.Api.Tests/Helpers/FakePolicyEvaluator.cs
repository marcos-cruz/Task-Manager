using System.Security.Claims;

using Bigai.TaskManager.Domain.Projects.Constants;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Bigai.TaskManager.Api.Tests.Helpers;

public class FakePolicyEvaluator : IPolicyEvaluator
{
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        var claimsPincipal = new ClaimsPrincipal();
        int userId = 101;

        claimsPincipal.AddIdentity(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, TaskManagerRoles.Manager),
                new Claim(ClaimTypes.Role, TaskManagerRoles.User),
                new Claim("UserId", $"{userId}"),
            })
        );

        AuthenticationTicket ticket = new AuthenticationTicket(claimsPincipal, "Test");
        AuthenticateResult result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        var result = PolicyAuthorizationResult.Success();

        return Task.FromResult(result);
    }
}