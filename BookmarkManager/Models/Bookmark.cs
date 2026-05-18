using Microsoft.AspNetCore.Identity;

namespace BookmarkManager.Models
{
    public class Bookmark
    {
        public int Id
        {
            get; set;
        }
        public string Url
        {
            get; set;
        }
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