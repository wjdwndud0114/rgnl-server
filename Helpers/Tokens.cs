using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rgnl_server.Auth;
using rgnl_server.Models;
using rgnl_server.Models.Resources;

namespace rgnl_server.Helpers
{
    public class Tokens
    {
        public static async Task<LoginResponseResource> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerSettings serializerSettings)
        {
            var response = new LoginResponseResource
            {
                Id = int.Parse(identity.Claims.Single(c => c.Type == Constants.Strings.JwtClaimIdentifiers.Id).Value),
                AuthToken = await jwtFactory.GenerateEncodedToken(userName, identity),
                ExpiresIn = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return response;
        }
    }
}