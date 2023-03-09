using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models
{
    public class Task : ObservableObject
    {
        #region Json Properties
        [JsonProperty("_id")]
        public string Id { get; set; }

        //[JsonProperty("projectId")]
        //public string ProjectId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        public Project Project { get; set; }
        [JsonProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        public string visibility { get; set; }
        public int priority { get; set; }
        public Assignedmember[] assignedMembers { get; set; }
        public bool isDone { get; set; }
        public Workspace workspace { get; set; }
        public Board board { get; set; }
        public Tasklist tasklist { get; set; }
        public Permission permission { get; set; }
        public string reminder { get; set; }
        #endregion

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                SetProperty(ref _isActive, value);
                if (Project != null)
                    Project.IsActive = _isActive;
            }
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


    public class Board
    {
        public string _id { get; set; }
        public DateTime createdAt { get; set; }
        public string createdBy { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string color { get; set; }
        public string workspaceId { get; set; }
        public string projectId { get; set; }
        public string shortName { get; set; }
        public object[] attachments { get; set; }
        public string visibility { get; set; }
    }

    public class Tasklist
    {
        public string _id { get; set; }
        public DateTime createdAt { get; set; }
        public string createdBy { get; set; }
        public string title { get; set; }
        public string boardId { get; set; }
        public string projectId { get; set; }
        public string workspaceId { get; set; }
        public string shortName { get; set; }
        public string visibility { get; set; }
        public int priority { get; set; }
    }

    public class Assignedmember
    {
        public string guid { get; set; }
        public DateTime createdAt { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public Status status { get; set; }
        public string realm { get; set; }
        public string cardNumber { get; set; }
        public string avatar { get; set; }
        public Permissions permissions { get; set; }
        public Organization organization { get; set; }
        public string companyName { get; set; }
        public string unitName { get; set; }
    }

    public class Status
    {
        public bool trash { get; set; }
        public bool suspend { get; set; }
        public bool active { get; set; }
    }

    public class Permissions
    {
        public Transaction transaction { get; set; }
    }

    public class Transaction
    {
        public string[] view { get; set; }
        public string[] edit { get; set; }
    }

    public class Organization
    {
        public string _id { get; set; }
        public DateTime createdAt { get; set; }
        public string userId { get; set; }
        public string companyId { get; set; }
        public string unitId { get; set; }
        public string role { get; set; }
        public string jobTitle { get; set; }
        public DateTime updatedAt { get; set; }
    }

}
