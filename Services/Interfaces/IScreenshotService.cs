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
    public interface IScreenshotService
    {
    

        [Post("/screenshot")]
        Task<ApiResponse<object>> Screenshot([Body(BodySerializationMethod.Serialized)] ScreenshotData payload);
    }
}
