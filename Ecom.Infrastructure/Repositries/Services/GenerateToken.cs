using Ecom.Core.Entites;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries.Services
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public GenerateToken(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GetAndCreateToken(ApplicationUser applicationUser)
        {
            #region Claims
            List<Claim> claims = new List<Claim>()
            {
                new Claim (ClaimTypes.Name,applicationUser.UserName),
                new Claim(ClaimTypes.Email,applicationUser.Email)
            };
            var Roles = await _userManager.GetRolesAsync(applicationUser);
            foreach (var RoleName in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, RoleName));

            }
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            #endregion

            #region SigningCredentials
            var SecretKey = Encoding.UTF8.GetBytes(_configuration["Token:Secret"]);

           SymmetricSecurityKey key = new SymmetricSecurityKey(SecretKey);  

           SigningCredentials  signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            #endregion

            #region Design_Token
            SecurityTokenDescriptor TokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                Issuer = _configuration["Token:Issure"],
                SigningCredentials = signingCredentials,
                NotBefore = DateTime.Now,
            };
            #endregion

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(TokenDescriptor);

            return handler.WriteToken(token);
        }
    }
}
