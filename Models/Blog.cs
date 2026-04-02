using System;
using System.ComponentModel.DataAnnotations;

namespace WarbandOfTheSpiritborn.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string BlogName { get; set; } = string.Empty;

        [Required]
        public string BlogPost { get; set; } = string.Empty;

        [Required]
        public string BlogAuthor { get; set; } = string.Empty;

        public DateTime ArticleDate { get; set; } = DateTime.UtcNow;

        public string? Comment { get; set; }

        public string? Reply { get; set; }
    }
}
