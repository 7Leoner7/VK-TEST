using System.ComponentModel.DataAnnotations;

namespace VK_TEST.VK_T_Objects.Models
{
    public class AuthenticateModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Login { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
