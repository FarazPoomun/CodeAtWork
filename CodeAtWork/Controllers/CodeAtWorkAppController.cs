using CodeAtWork.BL;
using CodeAtWork.Models.Session;
using System;
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
    }
}