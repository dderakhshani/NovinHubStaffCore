using NovinDevHubStaffCore.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace NovinDevHubStaffCore.Services
{
   public interface IAuthService
    {
        [Post("/auth/realms/nhc/protocol/openid-connect/token/")]
        public Task<ApiResponse<LoginResult>> Authenticate([Body(BodySerializationMethod.UrlEncoded)] LoginRequest payload);
    }
}
