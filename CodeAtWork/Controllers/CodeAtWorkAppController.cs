using CodeAtWork.BL;
using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using System;
using System.Collections.Generic;
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
            ViewBag.FirstName = userInfo.FirstName;
            ViewBag.Initial = userInfo.FirstName.ToCharArray()[0];
            ViewBag.RecommendedWatch = codeAtWorkAppBL.GetRecommendedVids(userInfo.UserId);

            return View();
        }

        public HtmlString GetFilteredVideos(int userChannelId, string filterBy)
        {
            return codeAtWorkAppBL.SearchVid(filterBy, false);
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

            ViewBag.ChannelsTabData = codeAtWorkAppBL.GetChannelList(userInfo.UserId);

            return View();
        }

        [HttpGet]
        public ActionResult AppPlayer(Guid playById)
        {
            ViewBag.VidFrame = codeAtWorkAppBL.PlayVideoById(playById);

            var GetVideoInfo = codeAtWorkAppBL.GetVideoInfo(playById);

            ViewBag.VideoName = GetVideoInfo.VideoDescription;
            ViewBag.By = GetVideoInfo.VideoAuthor;

            return View();
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
            return  vidId is null? null : codeAtWorkAppBL.GetVideoChannels(new Guid(videoId), userInfo.UserId);
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
    }
}