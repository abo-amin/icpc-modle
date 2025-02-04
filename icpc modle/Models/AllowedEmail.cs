using System.ComponentModel.DataAnnotations;

namespace icpc_modle.Models
{
    public class AllowedEmail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
    }
}