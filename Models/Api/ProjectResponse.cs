using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Models.Api
{

    public class ProjectResponse
    {
        public Pagination pagination { get; set; }

        public List<Project> projects { get; set; }
    }

    public class Pagination
    {
        public int totalCount { get; set; }
    }
}
