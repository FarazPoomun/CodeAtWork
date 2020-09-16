using CodeAtWork.Common;
using CodeAtWork.DAL;
using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace CodeAtWork.BL
{
    public class CodeAtWorkAppBL
    {
        CodeAtWorkAppDAL dal;

        public CodeAtWorkAppBL()
        {
            dal = new CodeAtWorkAppDAL();
        }

        public HtmlString PlayVideoById(Guid playById)
        {
            return dal.PlayVideoById(playById).ConvertVidToHtmlString();
        }

        public HtmlString GetRecommendedVids(int userID)
        {
            //TO-DO based on selection made on interests, find proper recommendations
            var vidsStr = ConvertVidGridHTMLSting(dal.GetRecommendedVids(userID));
            return new HtmlString(vidsStr);
        }

        internal void BookMarkVideo(Guid videoId, int userId, bool isSelected)
        {
            dal.SaveNewBookMark(videoId, userId, isSelected);
        }

        public string ConvertVidGridHTMLSting(List<VideoRepository> vids)
        {
            string resultStr = "";
            vids.ForEach(v =>
            {
                resultStr += "<div>" +
                $"<div class=\"vidGrid\" style=\"background-image: url('../../Uploads/{v.VideoId}.jpg'); \">" +
                "<div class=\"innerScreenshot\">" +
                $"<svg onclick=\"BookMarkVid(this,'{v.VideoId}')\" role=\"img\" viewBox=\"0 0 24 24\"";

                if (v.IsBookMarked)
                    resultStr += $" class=\"bookmarkingSvg bookmarkSelected\" >";

                else
                    resultStr += $" class=\"bookmarkingSvg bookmarkUnselected\" >";

                resultStr += $"<path fill=\"currentColor\" fill-rule=\"evenodd\" d=\"M17.501 2H9C6.794 2 5 3.795 5 6v17l5.5-6 5.5 6V10h4a1 1 0 001-1V6c0-2.205-1.292-4-3.499-4zM14 6v12l-3.5-4L7 18V6c0-1.104.897-2 2-2h5.536A3.99 3.99 0 0014 6zm5 2h-3V6c0-1.104.398-2 1.501-2C18.603 4 19 4.896 19 6v2z\"></path></svg> " +
                $"<i onclick=\"OpenPlayer('{v.VideoId}')\" class=\"far fa-play-circle\"></i>" +
                "</div>" +
                "</div>" +
                $"<p class=\"vidDesc\" onclick=\"OpenPlayer('{v.VideoId}')\"> {v.VideoDescription} </p>" +
                $"<p class=\"vidBy\">{v.VideoAuthor}</p>" +
                "</div>";
            });

            return resultStr;
        }
    }
}