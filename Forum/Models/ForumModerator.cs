using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Forum.Models
{
    // Model pośredniczący między Forum a Moderatorem (do przypisywania moderacji do forów)
    public class ForumModerator
    {
        public int ForumId { get; set; }
        public Forum Forum { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}