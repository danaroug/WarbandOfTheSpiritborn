using System;
using System.ComponentModel.DataAnnotations;

namespace WarbandOfTheSpiritborn.Models
{
    public class Events
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string EventName { get; set; } = string.Empty;

        [Display(Name = "Info")]
        public string EventInfo { get; set; } = string.Empty;

        [Display(Name = "Time")]
        public string Time { get; set; } = string.Empty;

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}
