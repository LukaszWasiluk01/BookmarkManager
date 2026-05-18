using Microsoft.AspNetCore.Identity;

namespace BookmarkManager.Models
{
    public class Report
    {
        public int Id
        {
            get; set;
        }
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