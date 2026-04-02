using System;
using System.ComponentModel.DataAnnotations;

namespace WarbandOfTheSpiritborn.Models
{
    public class Builds
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Profession is required")]
        public string Profession { get; set; } = string.Empty;

        public string BuildName { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string BuildAuthor { get; set; } = string.Empty;
        public string Item { get; set; } = string.Empty;
        public string Stat { get; set; } = string.Empty;
        public string WeaponSet { get; set; } = string.Empty;
        public string OtherItems { get; set; } = string.Empty;
        public string Rotation { get; set; } = string.Empty;
        public string MainSkills { get; set; } = string.Empty;
        public string SecondarySkills { get; set; } = string.Empty;

        public DateTime BuildDate { get; set; } = DateTime.UtcNow;
    }
}
