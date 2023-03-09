using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models
{
    public class ScreenshotData
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("thumbUrl")]
        public string ThumbUrl { get; set; }
        [JsonProperty("createAt")]
        public string CreateAt { get; set; }
    }
}
