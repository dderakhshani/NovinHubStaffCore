using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NovinDevHubStaffCore.Models;
using NovinDevHubStaffCore.Models.Api;
using Refit;

namespace NovinDevHubStaffCore.Services
{
    interface IProjectService
    {
        [Get("/projects")]
        public Task<ApiResponse<GenericResult<ProjectResponse>>> Get();
    }
}
