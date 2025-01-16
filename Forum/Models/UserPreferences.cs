using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Forum.Models
{
    public class UserPreferences
    {
        [Key]
        public string UserId { get; set; }
        public int MessagesPerPage { get; set; }
    }
}