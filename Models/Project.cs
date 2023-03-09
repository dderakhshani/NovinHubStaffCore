using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models
{
    public class Project : ObservableObject
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("createdBy")]
        public Guid CreatedBy { get; set; }

        [JsonProperty("workspaceId")]
        public string WorkspaceId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        //[JsonProperty("isStar")]
        //public bool IsStar { get; set; }

        //[JsonProperty("watchers")]
        //public bool Watchers { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("attachments")]
        public string[] Attachments { get; set; }

        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonProperty("dueDate")]
        public DateTimeOffset DueDate { get; set; }

        [JsonProperty("wsObjId")]
        public string WsObjId { get; set; }

        [JsonProperty("workspace")]
        public Workspace Workspace { get; set; }

        [JsonProperty("permission")]
        public Permission Permission { get; set; }


        public List<Task> Tasks { get; set; }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        private bool _selected;
        public bool IsSelected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }
        private TimeSpan _totalDurationSeconds;
        public TimeSpan TotalDurationSeconds
        {
            get => _totalDurationSeconds;
            set => SetProperty(ref _totalDurationSeconds, value);
        }
        public void IncreaseTime()
        {
            TotalDurationSeconds = TotalDurationSeconds.Add(TimeSpan.FromSeconds(1));
        }

       
    }

    public partial class Permission
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("create")]
        public bool Create { get; set; }

        [JsonProperty("update")]
        public bool Update { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }

        [JsonProperty("invitation")]
        public bool Invitation { get; set; }
    }

    public partial class Workspace
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("categoryTitle")]
        public string CategoryTitle { get; set; }

        [JsonProperty("webSite")]
        public Uri WebSite { get; set; }

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("createdBy")]
        public Guid CreatedBy { get; set; }

        [JsonProperty("shortName")]
        public string ShortName { get; set; }

        [JsonProperty("isStar")]
        public object[] IsStar { get; set; }

        [JsonProperty("watchers")]
        public object[] Watchers { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }

}
