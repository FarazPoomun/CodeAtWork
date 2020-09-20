using CodeAtWork.BL;
using CodeAtWork.Models.Session;
using System;
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
            UserInfo mockinfo = new UserInfo()
            {
                UserId = 1

            };
            Session["UserInfo"] = mockinfo;

            //GetRecommended
            var userInfo = Session["UserInfo"] as UserInfo;
            ViewBag.RecommendedWatch = codeAtWorkAppBL.GetRecommendedVids(userInfo.UserId);

            return View();
        }
        public ActionResult Bookmarks()
        {
            var userInfo = Session["UserInfo"] as UserInfo;

            ViewBag.BookkMarkedVids = codeAtWorkAppBL.GetBookMarkedVideos(userInfo.UserId);

            return View();
        }

        [HttpGet]
        public ActionResult AppPlayer(Guid playById)
        {
            ViewBag.VidFrame = codeAtWorkAppBL.PlayVideoById(playById);
            return View();
        }

        public void BookMarkVideo(string videoId, bool isSelected)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            //TO-DO Get appid from session and pass through
            codeAtWorkAppBL.BookMarkVideo(new Guid(videoId), userInfo.UserId, isSelected);
        }

        public HtmlString AddAndLinkChannel(string channelName, string videoId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            //TO-DO Get appid from session and pass through
            codeAtWorkAppBL.AddAndLinkChannel(new Guid(videoId), userInfo.UserId, channelName);
            return codeAtWorkAppBL.GetVideoChannels(new Guid(videoId), userInfo.UserId);
        }

        public HtmlString GetVideoChannels(string vidId)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            return codeAtWorkAppBL.GetVideoChannels(new Guid(vidId), userInfo.UserId);
        }

        public HtmlString AddOrRemoveChannelFromVid(string videoId, int channelId, bool isSelected)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAppBL.AddOrRemoveChannelFromVid(new Guid(videoId), channelId, isSelected, userInfo.UserId);

            return codeAtWorkAppBL.GetVideoChannels(new Guid(videoId), userInfo.UserId);

        }

        public HtmlString SearchVideo(string searchedTxt)
        {
            return codeAtWorkAppBL.SearchVid(searchedTxt);
        }
    }
}