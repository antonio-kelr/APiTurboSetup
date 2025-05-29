using System.ComponentModel.DataAnnotations;

namespace APiTurboSetup.Models
{
    public class GoogleAuthModel
    {
        [Required]
        public string token { get; set; }
    }
} 