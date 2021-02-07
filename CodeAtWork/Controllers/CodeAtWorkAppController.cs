using CodeAtWork.BL;
using CodeAtWork.Models;
using CodeAtWork.Models.Misc;
using CodeAtWork.Models.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAppController : Controller
    {
        readonly CodeAtWorkAppBL codeAtWorkAppBL = new CodeAtWorkAppBL();

        public CodeAtWorkAppController()
        {

        }

        // GET: CodeAtWorkApp
        public ActionResult Home()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }

            //GetRecommended
            var userInfo = Session["UserInfo"] as UserInfo;
            var DurationForWeekInMin = codeAtWorkAppBL.GetDurationForWeek(userInfo.UserId);
            ViewBag.DurationForWeek = $"{DurationForWeekInMin}min";
            ViewBag.ProgressForWeekHtml = codeAtWorkAppBL.ConvertToThisWeekProgress(DurationForWeekInMin);

            ViewBag.FirstName = userInfo.FirstName;
            ViewBag.Initial = userInfo.FirstName.ToCharArray()[0];
            ViewBag.RecommendedWatch = codeAtWorkAppBL.GetRecommendedVids(userInfo.UserId);
            var recents = codeAtWorkAppBL.GetRecentViewedVids(userInfo.UserId);
            ViewBag.RecentViews = recents;
            return View();
        }

        public HtmlString GetFilteredVideos(int userChannelId, string filterBy)
        {
            return codeAtWorkAppBL.SearchVid(filterBy, false);
        }
        public HtmlString GetFilteredPaths(int userChannelId, string filterBy)
        {
            return codeAtWorkAppBL.SearchPaths(filterBy, false);
        }

        public ActionResult ChannelDetail(int channelId)
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }
            //GetRecommended
            var userInfo = Session["UserInfo"] as UserInfo;

            var channelInfo = codeAtWorkAppBL.GetChannelInfo(channelId);

            ViewBag.ChannelName = channelInfo.ChannelName;
            ViewBag.CreatedBy = channelInfo.CreatedBy;
            ViewBag.UserChannelId = channelInfo.UserChannelId;
            ViewBag.IsShared = channelInfo.IsShared;
            ViewBag.ChannelVideos = GetChannelVideos(channelId);

            return View();
        }

        public HtmlString GetChannelVideos(int channelId)
        {
            return codeAtWorkAppBL.GetChannelVideos(channelId);
        }

        public ActionResult Paths()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }

            ViewBag.Paths = GetPathsPane(null);

            return View();
        }

        public HtmlString GetPathsPane(CategoryEnum? category, int tabId = 1)
        {
            //tabId = 1 == All /  = 2 == Following
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetAllPaths(userInfo.UserId, category, tabId);
        }

        public HtmlString GetPathsPanePerChannelId(int channelId)
        {
            //tabId = 1 == All /  = 2 == Following
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetPathsPanePerChannelId(userInfo.UserId, channelId);
        }

        public void AddPathToChannel(int channelId, int pathId)
        {
            codeAtWorkAppBL.AddPathToChannel(channelId, pathId);
        }

        public ActionResult Bookmarks()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }
            var userInfo = Session["UserInfo"] as UserInfo;

            ViewBag.BookMarkedVids = codeAtWorkAppBL.GetBookMarkedVideos(userInfo.UserId);

            return View();
        }
        public ActionResult Channels()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }
            var userInfo = Session["UserInfo"] as UserInfo;

            ViewBag.ChannelsTabData = GetChannelList();

            return View();
        }

        public HtmlString GetChannelList(int isShared = 0)
        {
            var userInfo = Session["UserInfo"] as UserInfo;

            return codeAtWorkAppBL.GetChannelList(userInfo.UserId, isShared);
        }

        public JsonResult SubscribeUserToChannel(int channelId, string email)
        {
            Models.Misc.ValidationResult result = new Models.Misc.ValidationResult();

            if (!new EmailAddressAttribute().IsValid(email))
            {
                result.HasValidationFailed = true;
                result.ValidationMsg = "Invalid Email Format";
            }
            else
            {
                var newId = codeAtWorkAppBL.SubscribeUserToChannel(channelId, email);
                if (newId is null)
                {
                    result.HasValidationFailed = true;
                    result.ValidationMsg = "Duplicate Email";
                }
                else if (newId == -1)
                {
                    result.HasValidationFailed = true;
                    result.ValidationMsg = "Invalid User";
                }
                else
                {
                    result.HasValidationFailed = false;
                    result.AdditionalMsg = codeAtWorkAppBL.ConvertToSubscribedPill(newId, email);
                }
            }
            return Json(result);
        }

        public HtmlString GetSubscribeUserToChannel(int channelId)
        {
            return codeAtWorkAppBL.GetSubscribeUserToChannel(channelId);
        }

        public void UnsubscribeUserToChannel(int channelSubscribedUserId)
        {
            codeAtWorkAppBL.UnsubscribeUserToChannel(channelSubscribedUserId);
        }

        [HttpGet]
        public ActionResult AppPlayer(Guid playById)
        {
            ViewBag.VidFrame = codeAtWorkAppBL.PlayVideoById(playById);

            var videoInfo = codeAtWorkAppBL.GetVideoInfo(playById);

            ViewBag.VideoName = videoInfo.VideoDescription;
            ViewBag.By = videoInfo.VideoAuthor;
            ViewBag.VideoId = playById;
            ViewBag.SeekTo = videoInfo.SeekTo;

            return View();
        }


        public void CaptureVideoTime(Guid videoId, float time, int IsFinished)
        {
            try
            {
                var userInfo = Session["UserInfo"] as UserInfo;

                codeAtWorkAppBL.CaptureTime(videoId, time, IsFinished, userInfo.UserId);
            }
            catch
            {

            }
        }

        public HtmlString GetNextRecommendedWatch(Guid videoId)
        {
            return codeAtWorkAppBL.GetNextRecommendedWatch(videoId);
        }

        public void BookMarkVideo(string videoId, bool isSelected)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            //TO-DO Get appid from session and pass through
            codeAtWorkAppBL.BookMarkVideo(new Guid(videoId), userInfo.UserId, isSelected);
        }

        public HtmlString AddAndLinkChannel(string channelName, string videoId = null)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            //TO-DO Get appid from session and pass through
            Guid? vidId;

            if (videoId is null)
            {
                vidId = null;
            }
            else
            {
                vidId = new Guid(videoId);
            }

            codeAtWorkAppBL.AddAndLinkChannel(vidId, userInfo.UserId, channelName);
            return vidId is null ? null : codeAtWorkAppBL.GetVideoChannels(new Guid(videoId), userInfo.UserId);
        }

        public HtmlString AddAndLinkChannelToPath(string channelName, int pathId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            //TO-DO Get appid from session and pass through

            codeAtWorkAppBL.AddAndLinkChannelToPath(pathId, userInfo.UserId, channelName);
            return GetPathChannels(pathId);
        }

        public HtmlString GetPathChannels(int pathId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetPathChannels(pathId, userInfo.UserId);
        }

        public HtmlString GetVideoChannels(string vidId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetVideoChannels(new Guid(vidId), userInfo.UserId);
        }

        public void AddVideoToChannel(string videoId, int channelId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.AddOrRemoveChannelFromVid(new Guid(videoId), channelId, true, userInfo.UserId);
        }

        public HtmlString AddOrRemoveChannelFromVid(string videoId, int channelId, bool isSelected)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.AddOrRemoveChannelFromVid(new Guid(videoId), channelId, isSelected, userInfo.UserId);

            return codeAtWorkAppBL.GetVideoChannels(new Guid(videoId), userInfo.UserId);
        }

        public HtmlString AddOrRemoveChannelFromPath(int pathId, int channelId, bool isSelected)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.AddOrRemovePathChannelFromVid(pathId, channelId, isSelected, userInfo.UserId);

            return codeAtWorkAppBL.GetPathChannels(pathId, userInfo.UserId);
        }

        public void DeleteChannels(List<int> channelIdsToDelete)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.DeleteChannels(channelIdsToDelete);
        }

        public void UpdateIsShared(int UserChannelId, bool isShared)
        {
            codeAtWorkAppBL.UpdateIsShared(UserChannelId, isShared);
        }

        public HtmlString SearchVideo(string searchedTxt)
        {
            return codeAtWorkAppBL.SearchVid(searchedTxt, true);
        }

        public ActionResult Interests()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }
            var userInfo = Session["UserInfo"] as UserInfo;
            ViewBag.TopicPills = codeAtWorkAppBL.GetTopicsByCategoryName(userInfo.UserId);
            return View();
        }

        public HtmlString GetTopicsByCategoryName(string CatergoryName)
        {
            var userInfo = Session["UserInfo"] as UserInfo;

            return codeAtWorkAppBL.GetTopicsByCategoryName(userInfo.UserId, CatergoryName);
        }

        public void UpdateInterestTopics(List<InterestCategoryTopicToBeSaved> InterestTopics)
        {

            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.SaveTopics(InterestTopics, userInfo.UserId);
        }

        #region Path Details
        public ActionResult PathDetails(int pathId)
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }
            var userInfo = Session["UserInfo"] as UserInfo;
            ViewBag.PathId = pathId;

            ViewBag.Details = codeAtWorkAppBL.GetPathDetail(pathId);

            return View();
        }
        #endregion

        public HtmlString GetQuestionnaireForVid(Guid VidId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetQuestionnaireForVid(VidId, userInfo.UserId);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login", "CodeAtWork");
        }

    }
}