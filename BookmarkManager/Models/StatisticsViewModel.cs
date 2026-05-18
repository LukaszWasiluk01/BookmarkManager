namespace BookmarkManager.Models
{
    public class StatisticsViewModel
    {
        public int TotalUsers
        {
            get; set;
        }
        public int TotalBookmarks
        {
            get; set;
        }
        public int TotalCategories
        {
            get; set;
        }
        public string MostPopularCategory
        {
            get; set;
        }
        public string MostActiveUser
        {
            get; set;
        }
    }
}