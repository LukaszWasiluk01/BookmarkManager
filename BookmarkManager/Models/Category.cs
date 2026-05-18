using System.ComponentModel.DataAnnotations;

namespace BookmarkManager.Models
{
    public class Category
    {
        public int Id
        {
            get; set;
        }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
        public string Name
        {
            get; set;
        }

        public ICollection<Bookmark> Bookmarks
        {
            get; set;
        }
    }
}