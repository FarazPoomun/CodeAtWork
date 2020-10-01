using CodeAtWork.DAL;
using CodeAtWork.Models;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class CodeAtWorkAdminBL
    {
        CodeAtWorkAdminDAL dal;

        public CodeAtWorkAdminBL()
        {
            dal = new CodeAtWorkAdminDAL();
        }

        public HtmlString GetTopics()
        {
            var topics = dal.GetTopics();

            return ConvertTopicsToSelectOptions(topics);

        }

        private HtmlString ConvertTopicsToSelectOptions(List<InterestCatergoryTopic> topics)
        {
            string result = "";
            topics.ForEach(t =>
            {
                result += $"<option value=\"{t.InterestCategoryTopicId}\">{t.Name}</option>";
            });

            return new HtmlString(result);
        }

        public Guid SaveNewVid(CreateVid vid)
        {
            return dal.SaveNewVideo(vid);
        }
    }
}