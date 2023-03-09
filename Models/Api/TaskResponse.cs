using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models.Api
{
    class TaskResponse
    {
        public Pagination pagination { get; set; }

        public List<Task> tasks { get; set; }
    }
}
