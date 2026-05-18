using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookmarkManager.Models
{
    public class Bookmark
    {
        public int Id
        {
            get; set;
        }

        [Required(ErrorMessage = "Adres URL jest wymagany.")]
        [Url(ErrorMessage = "Podano niepoprawny format adresu URL.")]
        public string Url
        {
            get; set;
        }

        [Required(ErrorMessage = "Opis zakładki jest wymagany.")]
        public string Description
        {
            get; set;
        }

        public DateTime CreatedAt
        {
            get; set;
        }

        public int CategoryId
        {
            get; set;
        }
        public Category Category
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