namespace CodeAtWork.Models
{
    public class InterestCatergoryTopic
    {
        public int InterestCategoryTopicId { get; set; }
        public int InterestCategoryId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

    public class InterestCategoryTopicToBeSaved
    {
        public int InterestCategoryTopicId { get; set; }
        public string InterestCategoryName { get; set; }
        public bool IsSelected { get; set; }
    }
}