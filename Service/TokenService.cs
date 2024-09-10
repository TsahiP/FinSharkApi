using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // Create a list of claims for the user
            var claims = new List<Claim>{
               new Claim(JwtRegisteredClaimNames.Email,user.Email),
               new Claim(JwtRegisteredClaimNames.GivenName,user.UserName)
            };

            // Create signing credentials using the symmetric security key
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create a token descriptor with the specified claims, expiration date, signing credentials, issuer, and audience
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            // Create a new instance of JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create a token based on the token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Write the token as a string
            return tokenHandler.WriteToken(token);
        }
    }
}