using System.ComponentModel.DataAnnotations;

namespace VirtualShopMinimalAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string? NomeUsuario { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? Cpf { get; set; }

        public string? Senha { get; set; }

        public bool IsAdmin { get; set; }
    }
}