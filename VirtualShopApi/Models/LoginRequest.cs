using System.ComponentModel.DataAnnotations;

namespace LojaVirtualAPI.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}