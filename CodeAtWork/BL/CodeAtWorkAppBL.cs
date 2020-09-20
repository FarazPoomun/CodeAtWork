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

        internal HtmlString SearchVid(string searchedTxt)
        {
            return new HtmlString(ConvertVidGridHTMLSting(dal.SearchVid(searchedTxt), false));
        }

        internal HtmlString GetBookMarkedVideos(int userId)
        {
            //TO-DO based on selection made on interests, find proper recommendations
            var vidsStr = ConvertVidGridHTMLSting(dal.GetBookmarkedVideos(userId));
            return new HtmlString(vidsStr);
        }

        internal void AddAndLinkChannel(Guid videoId, int userId, string channelName)
        {
            dal.AddAndLinkChannel(videoId, userId, channelName);
        }

        public string ConvertVidGridHTMLSting(List<VideoRepository> vids, bool withSVG = true)
        {
            string resultStr = "";
            vids.ForEach(v =>
            {
                resultStr += "<div>" +
                $"<div class=\"vidGrid\" style=\"background-image: url('../../Uploads/{v.VideoId}.jpg'); \">" +
                "<div class=\"innerScreenshot\">";
                if (withSVG)
                {
                    resultStr += $"<svg id=\"opt_{v.VideoId}\" role=\"img\" viewBox=\"0 0 24 24\" class=\"optSvg\" onclick=\"toggleOptionMenu('{v.VideoId}')\"><path fill=\"currentColor\" " +
                    $"fill-rule=\"evenodd\" d=\"M6 14.5a2 2 0 110-4 2 2 0 010 4zm12 0a2 2 0 110-4 2 2 0 010 4zm-6 0a2 2 0 110-4 2 2 0 010 4z\"></path></svg>" +
                    $"<div class=\"optMenu\" id = \"optMenu_{v.VideoId}\">" +
                    $"  <ul class=\"optMenuList\">" +
                                "<li>Play</li>" +
                               "<li>Add To Bookmark</li>" +
                               $"<li onclick=\"showOptChannel('{v.VideoId}')\" id=\"optMenuChannelBtn_{v.VideoId}\">Add To Channel <i class=\"fas fa-caret-right optMenuListRight\"></i></li>" +
                            "</ul></div>" +
                            $"<div class=\"optMenuChannel\" id=\"optMenuChannel_{v.VideoId}\">" +
                            $"<div class=\"optMenuChannelDiv1\">" +
                                $"<input type=\"text\" placeholder=\"Create New Channel\" class=\"ChannelAddBar\" id=\"ChannelAddBar_{v.VideoId}\" onkeyup=\"EnableAddChannelBtn(this, '{v.VideoId}')\">" +
                               $" <button type=\"submit\" class=\"ChannelAddBtn\" id=\"ChannelAddBtn_{v.VideoId}\" onclick=\"AddNewChannel('{v.VideoId}')\"><i class=\"fas fa-plus\"></i></button>" +
                           " </div> " +
                            "<div style=\"padding-bottom: 5%;\">" +
                                "<ul class=\"optMenuList\">" +
                                "    <li>Play<i class=\"fas fa-check-circle optMenuListRight channelUnselected\"></i></li>" +
                                "    <li>Add To Bookmark <i class=\"fas fa-check-circle optMenuListRight channelSelected\"></i></li>" +
                                "    <li>Add To Channel </li>" +
                              "  </ul>" +
                            "</div>" +
                            $"</div>" +
                    $"<svg onclick=\"BookMarkVid(this,'{v.VideoId}')\" role=\"img\" viewBox=\"0 0 24 24\"";

                    if (v.IsBookMarked)
                        resultStr += $" class=\"bookmarkingSvg bookmarkSelected\" >";

                    else
                        resultStr += $" class=\"bookmarkingSvg bookmarkUnselected\" >";

                    resultStr += $"<path fill=\"currentColor\" fill-rule=\"evenodd\" d=\"M17.501 2H9C6.794 2 5 3.795 5 6v17l5.5-6 5.5 6V10h4a1 1 0 001-1V6c0-2.205-1.292-4-3.499-4zM14 6v12l-3.5-4L7 18V6c0-1.104.897-2 2-2h5.536A3.99 3.99 0 0014 6zm5 2h-3V6c0-1.104.398-2 1.501-2C18.603 4 19 4.896 19 6v2z\"></path></svg> ";
                }
                resultStr += $"<i onclick=\"OpenPlayer('{v.VideoId}')\" class=\"far fa-play-circle\"></i>" +
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