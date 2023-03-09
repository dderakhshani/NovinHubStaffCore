using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Core.Activities
{
    public class ProccessActivity
    {
        public string ProcessName { get; set; }
        public int UseDurationSeconds { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public int ProcessId { get; set; }
        public string WindowTitle { get; set; }
    }
}
