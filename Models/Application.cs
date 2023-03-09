using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models
{
    public class Application
    {
        #region Json Properties
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("taskId")]
        public string TaskId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }


        [JsonProperty("startTime")]
        public string StartTime { get; set; }

        [JsonProperty("tracked")]
        public int Tracked { get; set; }

        #endregion
    }
}
