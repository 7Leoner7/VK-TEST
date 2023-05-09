using System.ComponentModel.DataAnnotations;

namespace VK_TEST.VK_T_Objects.Models
{
    public class UserModel
    {
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Login { get; set; }
        
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        public string Password { get; set; }

        [Required]
        public DateTime Created_Date { get; set; }

        [Required]
        public int UserGroupID { get; set; }
        
        [Required]
        public int UserStateID { get; set; }
    }
}
