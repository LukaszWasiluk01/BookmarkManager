using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookmarkManager.Models
{
    public class Report
    {
        public int Id
        {
            get; set;
        }

        [Required(ErrorMessage = "Treść zgłoszenia jest wymagana.")]
        public string Content
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }

        public string UserId
        {
            get; set;
        }
        public IdentityUser User
        {
            get; set;
        }
    }
}