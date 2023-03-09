using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Core.Activities
{
    public class BrowserActivity
    {
        public BrowserActivity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string BrowserName { get; set; }
        public string Url { get; set; }
    }
}
