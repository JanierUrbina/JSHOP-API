using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_CHATN.Modelo;
using Microsoft.IdentityModel.Tokens;

namespace Api_ChatN.Helpers;
public interface IAuthService
{
    public record validateLogin(string username, string password);
}
