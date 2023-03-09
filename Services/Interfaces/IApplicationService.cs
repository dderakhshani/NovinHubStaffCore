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
    public interface IApplicationService
    {
        [Post("/application")]
        public Task<ApiResponse<GenericResult<Application>>> Post([Body] Application payload);
    }
}
