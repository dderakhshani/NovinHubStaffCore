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
    interface ITaskService
    {
        [Get("/tasks")]
        public Task<ApiResponse<GenericResult<TaskResponse>>> Get();

        [Patch("/tasks/done")]
        public Task<ApiResponse<GenericResult<string>>> Complete([Body] TaskDone taskId);
    }


}
