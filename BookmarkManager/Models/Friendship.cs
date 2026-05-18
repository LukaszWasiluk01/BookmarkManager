using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookmarkManager.Models
{
    public class Friendship
    {
        public int Id
        {
            get; set;
        }

        [Required]
        public string RequesterId
        {
            get; set;
        }

        [ForeignKey("RequesterId")]
        public IdentityUser Requester
        {
            get; set;
        }

        [Required]
        public string AddresseeId
        {
            get; set;
        }

        [ForeignKey("AddresseeId")]
        public IdentityUser Addressee
        {
            get; set;
        }

        public bool IsAccepted
        {
            get; set;
        }
    }
}