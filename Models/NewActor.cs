using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace movies.Models
{
    public class NewActor
    {
        [Required]
        [MaxLength(255)]
        public string Fullname { get; set; }

        [Required]
        public DateTimeOffset Birthdate { get; set; }

        [Required]
        [Display(Name="File")]
        public IFormFile Image { get; set; }
    }
}
