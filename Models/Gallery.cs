using System.ComponentModel.DataAnnotations;

namespace WarbandOfTheSpiritborn.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please choose an image")]
        public string Picture { get; set; } = string.Empty;
    }
}
