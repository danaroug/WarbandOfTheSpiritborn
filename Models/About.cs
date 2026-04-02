using System.ComponentModel.DataAnnotations;

namespace WarbandOfTheSpiritborn.Models
{
    public class About
    {
        public int Id { get; set; }

        [Required]
        public string AboutTitle { get; set; } = string.Empty;

        [Required]
        public string AboutText { get; set; } = string.Empty;
    }
}
