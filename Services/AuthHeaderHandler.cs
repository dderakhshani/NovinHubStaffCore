using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Services
{
    class AuthHeaderHandler :  HttpClientHandler
    {
        private readonly IAuthTokenStore authTokenStore;

        public AuthHeaderHandler(IAuthTokenStore authTokenStore)
        {
            this.authTokenStore = authTokenStore;
            
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token = "";
            var login = await authTokenStore.GetToken();

            if (login != null)
                if (login.ExpiresIn > 0)
                {
                    token = login.AccessToken;
                }
                else
                    token = login.RefreshToken;
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
