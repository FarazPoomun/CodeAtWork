using System.Collections.Generic;

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
        public int InterestCategoryId { get; private set; }
        public string InterestCategoryName { get; set; }
        public bool IsSelected { get; set; }

        public void SetInterestCategoryId(Dictionary<string, int> topics)
        {
           if(topics.TryGetValue(InterestCategoryName, out int val))
            {
                InterestCategoryId = val;
            }
        }
    }
}