using CodeAtWork.DAL;
using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
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

        internal dynamic GetUsers(bool isActive)
        {
            return ConvertToTableHtml(dal.GetUsersList(isActive));
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


        public HtmlString ConvertToTableHtml(List<FullUserDetail> UC)
        {
            string result = "";
            UC.ForEach(u =>
            {
                result += $"<tr id=\"channelRow_{u.AppUserId}\">" +
                $"<td><input type=\"checkbox\" onchange=\"softDeleteRow(this, {u.AppUserId})\" /></td>" +
                $"<td >{u.FirstName}</td> " +
               $"<td>({u.LastName}) Videos</td> " +
              $"<td>({u.Email}) Paths</td> " +
                $"<td>By {u.Company}</td> " +
                "</tr> ";
            });

            return new HtmlString(result);
        }
    }
}