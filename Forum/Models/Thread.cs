using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Forum.Models
{
    public class Thread
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; }
        public string UserName { get; set; }

        public int ForumId { get; set; }
        public Forum Forum { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        public int Views { get; set; }
        public bool isPinned { get; set; }
    }
}