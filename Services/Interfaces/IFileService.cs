using NovinDevHubStaffCore.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Services
{
    public interface IFileService
    {
        [Multipart]
        [Post("/file/files")]
        Task<ApiResponse<GenericResult<string>>> UploadPictureAsync([AliasAs("file")] StreamPart stream);
    }
}
