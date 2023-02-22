using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class GenreViewModel
    {
        public int Id { get; set; }

        [StringLength(500, MinimumLength = 1), Required]
        public string Name { get; set; }
    }
}
