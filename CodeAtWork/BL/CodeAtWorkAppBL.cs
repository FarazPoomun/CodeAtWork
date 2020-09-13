using CodeAtWork.Common;
using CodeAtWork.DAL;
using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public HtmlString GetRecommendedVids()
        {
            //TO-DO based on selection made on interests, find proper recommendations
            var vidsStr = ConvertVidGridHTMLSting(dal.GetRecommendedVids());
            return new HtmlString(vidsStr);
        }


        public string ConvertVidGridHTMLSting(List<VideoRepository> vids)
        {
            string resultStr = "";
            vids.ForEach(v => {
                resultStr += "<div>" +
                $"<div class=\"vidGrid\" style=\"background-image: url('../../Uploads/{v.VideoId}.jpg'); \">" +
                "<div class=\"innerScreenshot\">" +
                $"<i onclick=\"OpenPlayer('{v.VideoId}')\" class=\"far fa-play-circle\"></i>" +
                "</div>"+
                "</div>"+
                $"<p class=\"vidDesc\" onclick=\"OpenPlayer('{v.VideoId}')\"> {v.VideoDescription} </p>" +
                $"<p class=\"vidBy\">{v.VideoAuthor}</p>" +
                "</div>";
            });

            return resultStr;
        }
    }
}