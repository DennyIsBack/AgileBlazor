using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AgileBlazor.Client.Auth
{
    public class DemoAuthStateProvider : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //indicamos se o usuário esta autenticado e também os seus claims
            var usuario = new ClaimsIdentity(new List<Claim> { 
            new Claim(ClaimTypes.Name,"Anonymous")
            },"demo");

            return await Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(usuario)));
        }
    }
}
