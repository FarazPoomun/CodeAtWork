using CodeAtWork.DAL;
using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class CodeAtWorkBL
    {
        CodeAtWorkDAL dal;
        public CodeAtWorkBL()
        {
            dal = new CodeAtWorkDAL();
        }

        #region DB calls
        public HtmlString GetTopicsByCategoryName(string CategoryName = "Software Development")
        {
            return ConvertTopicsToHTMLSting(dal.GetTopicsByCategoryName( new List<string>() { CategoryName }));
        }

        public int ValidateLoginDetail(string loginId, string pwd)
        {
            return dal.ValidateLoginDetail(loginId, pwd);
        }


        public void SaveTopics(List<InterestCategoryTopicToBeSaved> topics)
        {
            Dictionary<int, bool> newTopicsToUpdate = new Dictionary<int, bool>();

            var existingTopics = dal.GetTopicsByCategoryName(topics.Select(z => z.InterestCategoryName).Distinct().ToList());

            topics.ForEach(
                t => {
                    var existingSelection = existingTopics
                    .FirstOrDefault(z => z.InterestCategoryTopicId == t.InterestCategoryTopicId && z.IsSelected != t.IsSelected);

                    if(existingSelection != null)
                    {
                        newTopicsToUpdate.Add(t.InterestCategoryTopicId, t.IsSelected);
                    }
                });

            if (newTopicsToUpdate.Any())
            {
                dal.SaveTopics(newTopicsToUpdate);
            }
        }

        #endregion

        private HtmlString ConvertTopicsToHTMLSting(List<InterestCatergoryTopic> topics) {

            var topicsString = "";
            topics.ForEach(t => {
                var isSubscribed = t.IsSelected ? "isSubscribed" : "isNotSubscribed";
                topicsString += $"<a id=\"Pill_{t.InterestCategoryTopicId}\" class=\"f6 br-pill ba ph3 pv2 mb2 dib {isSubscribed}\" onclick=\"subscribeTopic(this)\">{t.Name}</a>";
            });

            return new HtmlString(topicsString);
        }



    }
}