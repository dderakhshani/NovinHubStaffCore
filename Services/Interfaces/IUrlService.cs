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
    [Headers("Authorization: Bearer")]
    public interface IUrlService
    {
        [Post("/url")]
        public Task<ApiResponse<GenericResult<UrlActivity>>> Post([Body] UrlActivity payload);
    }
}
