using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Digger.Server
{
    public class TokenService
    {
        readonly IOptions<TokenServiceOptions> _options;

        public TokenService(IOptions<TokenServiceOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _options = options;
        }

        public string GenerateToken()
        {
            var now = DateTime.UtcNow;

            // Specifically add the iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim( "IsDiggerServer", "true", ClaimValueTypes.Boolean )
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Value.Expiration),
                signingCredentials: _options.Value.SigningCredentials);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class TokenServiceOptions
    {
        public SigningCredentials SigningCredentials { get; set; }

        public TimeSpan Expiration { get; set; }
    }
}
