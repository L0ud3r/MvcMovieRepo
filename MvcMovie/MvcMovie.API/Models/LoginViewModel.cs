using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class LoginViewModel
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$"), Required, StringLength(100)]
        public string? Email { get; set; }
        [StringLength(500, MinimumLength = 6), Required]
        public string? Password { get; set; }
    }
}
