using CodeAtWork.Common;
using CodeAtWork.DAL;
using CodeAtWork.Models;
using CodeAtWork.Models.Misc;
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

        internal void BookMarkVideo(Guid videoId, int userId, bool isSelected)
        {
            dal.SaveNewBookMark(videoId, userId, isSelected);
        }

        internal void AddPathToChannel(int channelId, int pathId)
        {
            dal.AddPathToChannel(channelId, pathId);
        }

        internal ChannelHeaderInfo GetChannelInfo(int channelId)
        {
            return dal.GetChannelInfo(channelId);
        }

        internal void UpdateIsShared(int userChannelId, bool isShared)
        {
            dal.UpdateIsShared(userChannelId, isShared);
        }

        internal VideoWithTime GetVideoInfo(Guid vidId)
        {
            return dal.GetVideoInfo(vidId);
        }

        internal void AddAndLinkChannel(Guid? videoId, int userId, string channelName)
        {
            dal.AddAndLinkChannel(videoId, userId, channelName);
        }

        internal void AddOrRemoveChannelFromVid(Guid videoId, int channelId, bool isSelected, int userId)
        {
            dal.AddOrRemoveChannelFromVid(videoId, channelId, isSelected, userId);
        }

        internal int GetDurationForWeek(int userId)
        {
            DateTime baseDate = DateTime.Now;

            var today = baseDate;
            DateTime thisWeekStart;
            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                thisWeekStart = baseDate.AddDays(-7);
            }
            else
            {
                thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            };

            thisWeekStart = new DateTime(thisWeekStart.Year, thisWeekStart.Month, thisWeekStart.Day, 0, 0, 0);
            return dal.GetDurationForWeek(userId, thisWeekStart);
        }

        internal void AddOrRemovePathChannelFromVid(int pathId, int channelId, bool isSelected, int userId)
        {
            dal.AddOrRemovePathChannelFromVid(pathId, channelId, isSelected, userId);
        }

        internal void DeleteChannels(List<int> channelIdsToDelete)
        {
            dal.DeleteChannels(channelIdsToDelete);
        }

        public void SaveTopics(List<InterestCategoryTopicToBeSaved> topics, int userId)
        {
            Dictionary<int, bool> newTopicsToUpdate = new Dictionary<int, bool>();

            var existingTopics = dal.GetTopicsByCategoryName(topics.Select(z => z.InterestCategoryName).Distinct().ToList(), userId);

            topics.ForEach(
                t =>
                {
                    var existingSelection = existingTopics
                    .FirstOrDefault(z => z.InterestCategoryTopicId == t.InterestCategoryTopicId && z.IsSelected != t.IsSelected);

                    if (existingSelection != null)
                    {
                        newTopicsToUpdate.Add(t.InterestCategoryTopicId, t.IsSelected);
                    }
                });

            if (newTopicsToUpdate.Any())
            {
                dal.SaveTopics(newTopicsToUpdate, userId);
            }
        }

        internal void AddAndLinkChannelToPath(int pathId, int userId, string channelName)
        {
            dal.AddAndLinkChannelToPath(pathId, userId, channelName);
        }

        internal PathDetail GetPathDetail(int pathId)
        {
            return dal.GetPathDetail(pathId);
        }

        internal void CaptureTime(Guid videoId, float time, int userId)
        {
            dal.CaptureTime(videoId, time, userId);
        }

        #region Htmlstring conversions

        public HtmlString ConvertToTableHtml(List<UserChannelWithCounts> UC)
        {
            string result = "";
            UC.ForEach(u =>
            {
                result += $"<tr id=\"channelRow_{u.UserChannelId}\">" +
                $"<td><input type=\"checkbox\" onchange=\"softDeleteRow(this, {u.UserChannelId})\" /></td>" +
                $"<td onclick=\"OpenChannelDetails({u.UserChannelId})\">{u.ChannelName}</td> " +
               $"<td onclick=\"OpenChannelDetails({u.UserChannelId})\">({u.VideoCount}) Videos</td> " +
              $"<td onclick=\"OpenChannelDetails({u.UserChannelId})\">({u.PathCount}) Paths</td> " +
                $"<td onclick=\"OpenChannelDetails({u.UserChannelId})\">By {u.CreatedBy}</td> " +
                "</tr> ";
            });

            return new HtmlString(result);
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

        public HtmlString GetChannelVideos(int channelId)
        {
            //TO-DO based on selection made on interests, find proper recommendations
            var vidsStr = ConvertVidGridHTMLSting(dal.GetChannelVideos(channelId));
            return new HtmlString(vidsStr);
        }

        internal HtmlString GetFilteredVideos(int userChannelId, int userId)
        {
            throw new NotImplementedException();
        }

        internal HtmlString GetChannelList(int userId)
        {
            return ConvertToTableHtml(dal.GetChannelLists(userId));
        }


        private HtmlString ConvertTopicsToHTMLSting(List<InterestCatergoryTopic> topics)
        {

            var topicsString = "";
            topics.ForEach(t =>
            {
                var isSubscribed = t.IsSelected ? "isSubscribed" : "isNotSubscribed";
                topicsString += $"<a id=\"Pill_{t.InterestCategoryTopicId}\" class=\"f6 br-pill ba ph3 pv2 mb2 dib {isSubscribed}\" onclick=\"subscribeTopic(this)\">{t.Name}</a>";
            });

            return new HtmlString(topicsString);
        }

        public string ConvertVidGridHTMLSting(List<VideoRepository> vids, bool withSVG = true, bool play = true)
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
                                $"<li onclick=\"OpenPlayer('{v.VideoId}')\">Play <i class=\"far fa-play-circle optMenuListRight\"></i></li>" +
                               "<li>Add To Bookmark <i class=\"fas fa-bookmark optMenuListRight\"></i></li>" +
                               $"<li onclick=\"showOptChannel('{v.VideoId}')\" id=\"optMenuChannelBtn_{v.VideoId}\">Add To Channel <i class=\"fas fa-caret-right optMenuListRight\"></i></li>" +
                            "</ul></div>" +
                            $"<div class=\"optMenuChannel\" id=\"optMenuChannel_{v.VideoId}\">" +
                            $"<div class=\"optMenuChannelDiv1\">" +
                                $"<input type=\"text\" placeholder=\"Add New Channel\" class=\"ChannelAddBar\" id=\"ChannelAddBar_{v.VideoId}\" onkeyup=\"EnableAddChannelBtn(this, '{v.VideoId}')\">" +
                               $" <button type=\"submit\" class=\"ChannelAddBtn\" id=\"ChannelAddBtn_{v.VideoId}\" onclick=\"AddNewChannel('{v.VideoId}')\"><i class=\"fas fa-plus\"></i></button>" +
                           " </div> " +
                            "<div style=\"padding-bottom: 5%;\">" +
                                $"<ul class=\"optMenuList ChannelList\" id =\"ChannelList_{v.VideoId}\">" +
                              "</ul>" +
                            "</div>" +
                            $"</div>" +
                    $"<svg onclick=\"BookMarkVid(this,'{v.VideoId}')\" role=\"img\" viewBox=\"0 0 24 24\"";

                    if (v.IsBookMarked)
                        resultStr += $" class=\"bookmarkingSvg bookmarkSelected\" >";

                    else
                        resultStr += $" class=\"bookmarkingSvg bookmarkUnselected\" >";

                    resultStr += $"<path fill=\"currentColor\" fill-rule=\"evenodd\" d=\"M17.501 2H9C6.794 2 5 3.795 5 6v17l5.5-6 5.5 6V10h4a1 1 0 001-1V6c0-2.205-1.292-4-3.499-4zM14 6v12l-3.5-4L7 18V6c0-1.104.897-2 2-2h5.536A3.99 3.99 0 0014 6zm5 2h-3V6c0-1.104.398-2 1.501-2C18.603 4 19 4.896 19 6v2z\"></path></svg> ";
                }
                if (play)
                {
                    resultStr += $"<i onclick=\"OpenPlayer('{v.VideoId}')\" class=\"far fa-play-circle\"></i>";
                }
                else
                {
                    resultStr += $"<i onclick=\"AddToChannel('{v.VideoId}')\" class=\"fas fa-plus-circle vidPlus\"></i>";
                }
                resultStr += "</div>" +
                "</div>" +
                $"<p class=\"vidDesc\" onclick=\"OpenPlayer('{v.VideoId}')\"> {v.VideoDescription} </p>" +
                $"<p class=\"vidBy\">{v.VideoAuthor} <i class=\"fas fa-circle dotSeperator\"></i> {v.Level.ToString()}</p>" +
                "</div>";
            });

            return resultStr;
        }

        public string ConvertToChannelLists(List<UserChannel> Channels, Guid videoId)
        {
            string result = "";

            Channels.ForEach(z =>
            {
                var selectedClass = z.IsSelectedForVid ? "channelSelected" : "channelUnselected";
                result += $"<li>{z.ChannelName} <i onclick=\"AddOrRemoveChannelFromVid(this, {z.UserChannelId}, '{videoId}')\" class=\"fas fa-check-circle optMenuListRight {selectedClass}\"></i></li>";
            });

            return result;
        }

        public string ConvertToPathChannelLists(List<UserChannel> Channels, int pathId)
        {
            string result = "";

            Channels.ForEach(z =>
            {
                var selectedClass = z.IsSelectedForVid ? "channelSelected" : "channelUnselected";
                result += $"<li>{z.ChannelName} <i onclick=\"AddOrRemoveChannelFromPath(this, {z.UserChannelId}, '{pathId}')\" class=\"fas fa-check-circle optMenuListRight {selectedClass}\"></i></li>";
            });

            return result;
        }

        internal HtmlString GetVideoChannels(Guid vidId, int userId)
        {
            return new HtmlString(ConvertToChannelLists(dal.GetVideoChannels(vidId, userId), vidId));
        }

        internal HtmlString GetPathChannels(int pathId, int userId)
        {
            return new HtmlString(ConvertToPathChannelLists(dal.GetPathChannels(pathId, userId), pathId));
        }
        public HtmlString GetTopicsByCategoryName(int userId, string CategoryName = "Software Development")
        {
            return ConvertTopicsToHTMLSting(dal.GetTopicsByCategoryName(new List<string>() { CategoryName }, userId));
        }
        internal HtmlString SearchVid(string searchedTxt, bool play)
        {
            return new HtmlString(ConvertVidGridHTMLSting(dal.SearchVid(searchedTxt), false, play));
        }

        internal HtmlString SearchPaths(string filterBy, bool v)
        {
            return ConvertToPathBlockHtml(dal.GetAllPathsFiltered(filterBy), withOpt: false, withAdd: true);
        }

        internal HtmlString GetBookMarkedVideos(int userId)
        {
            //TO-DO based on selection made on interests, find proper recommendations
            var vidsStr = ConvertVidGridHTMLSting(dal.GetBookmarkedVideos(userId), false);
            return new HtmlString(vidsStr);
        }


        internal HtmlString GetAllPaths(int userId, CategoryEnum? category, int tabId)
        {
            return ConvertToPathBlockHtml(dal.GetAllPaths(userId, category, tabId));
        }

        internal HtmlString GetPathsPanePerChannelId(int userId, int channelId)
        {
            return ConvertToPathBlockHtml(dal.GetPathsPanePerChannelId(channelId), false);
        }

        private HtmlString ConvertToPathBlockHtml(List<Path> list, bool withOpt = true, bool withAdd = false)
        {
            string result = "";
            list.ForEach(p =>
            {

                result += "  <div class=\"pathCard\">" +
                    $"<div class=\"outerImgDiv\" onclick=\"viewPath({p.PathId})\">" +
                        "<img class=\"pathCardImg\" src=\"/Design/Topics/CSharp.png\" />" +
                    "</div>" +
                    $"<div class=\"outerTextDiv\" onclick=\"viewPath({p.PathId})\">" +
                        $"<h2> {p.Name}</h2>" +
                        $"<p> <a class=\"PathVidCount\">14</a> Courses  <i class=\"fas fa-circle dotSeperator\"></i> {p.Level.ToString()} </p>" +
                    "</div>";

                if (withOpt)
                {
                    result += "<div>" +
                        $"<svg class=\"pathOpt\" onclick=\"openPathOptMenu({p.PathId})\" role=\"img\" viewBox=\"0 0 24 24\">" +
                            "<path fill=\"currentColor\" fill-rule=\"evenodd\" d=\"M6 14.5a2 2 0 110-4 2 2 0 010 4zm12 0a2 2 0 110-4 2 2 0 010 4zm-6 0a2 2 0 110-4 2 2 0 010 4z\"></path>" +
                        "</svg>" +
                        $"<div class=\"optMenuPath\" id=\"OptMenuPath_{p.PathId}\">" +
                            $"<p onclick=\"OpenAddPathToChannel({p.PathId})\" class =\"AddToChannelTxt\">Add to Channel</p>" +
                        "</div>" +
                        $"<div class=\"optMenuChannel\" id=\"OptMenuPathChannel_{p.PathId}\">" +
                            "<div class=\"optMenuChannelDiv1\">" +
                                $"<input type=\"text\" placeholder=\"Create New Channel\" class=\"ChannelAddBar\" id=\"ChannelAddBar_{p.PathId}\" onkeyup=\"EnableAddChannelBtn(this, {p.PathId})\">" +
                                $"<button type=\"submit\" class=\"ChannelAddBtn\" id=\"PathChannelAddBtn_{p.PathId}\" onclick=\"AddNewChannel({p.PathId})\"><i class=\"fas fa-plus\"></i></button>" +
                            "</div>" +
                            "<div style=\"padding-bottom: 5%;\">" +
                                $"<ul class=\"optMenuList ChannelList\" id=\"PathChannelList_{p.PathId}\"></ul>" +
                            "</div></div></div>";
                }
                if (withAdd)
                {
                    result += $"<div><i onclick=\"AddPathToChannel({p.PathId})\" class=\"fas fa-plus-circle pathPlus\"></i> </div>";
                }
                result += "</div>";
            });

            return new HtmlString(result);
        }

        public HtmlString ConvertToThisWeekProgress(int value)
        {
            string result = $@"<div class='bar actual' data-value='{value}' data -color='#0084bd'>
                <div class='label'> {value} MIN</div> </div>";
            return new HtmlString(result);
        }

        #endregion
    }
}