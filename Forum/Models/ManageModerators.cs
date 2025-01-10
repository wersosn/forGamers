using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forum.Models
{
    public class ManageModerators
    {
        public Forum Forum { get; set; }
        public List<ApplicationUser> AllUsers { get; set; }
        public List<string> AssignedModeratorIds { get; set; }
    }
}