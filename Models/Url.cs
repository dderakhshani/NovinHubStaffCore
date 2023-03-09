using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models
{
    public class UrlActivity
    {
        #region Json Properties
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("taskId")]
        public string TaskId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }


        [JsonProperty("fromTime")]
        public string FromTime { get; set; }
        [JsonProperty("toTime")]
        public string ToTime { get; set; }

        //[JsonProperty("tracked")]
        //public string Tracked { get; set; }

        #endregion
    }
}
