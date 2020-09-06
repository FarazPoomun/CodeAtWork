using CodeAtWork.DAL;
using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class InterestsBL
    {
        InterestDAL dal;
        public InterestsBL()
        {
            dal = new InterestDAL();
        }

        #region DB calls
        public HtmlString GetTopicsByCategoryName(string CategoryName = "Software Development")
        {
            return ConvertTopicsToHTMLSting(dal.GetTopicsByCategoryName(CategoryName));
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