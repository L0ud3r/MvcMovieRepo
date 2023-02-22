using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models
{
    public class UserViewModel : LoginViewModel
    {
        public int? Id { get; set; }
        [StringLength(500, MinimumLength = 1), Required]
        public string? Username { get; set; }
    }
}
