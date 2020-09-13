using CodeAtWork.BL;
using System;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAppController : Controller
    {
        readonly CodeAtWorkAppBL codeAtWorkAppBL = new CodeAtWorkAppBL();
        // GET: CodeAtWorkApp
        public ActionResult Home()
        {
            //GetRecommended
            ViewBag.RecommendedWatch = codeAtWorkAppBL.GetRecommendedVids();

            return View();
        }

        [HttpGet]
        public ActionResult AppPlayer(Guid playById)
        {
            ViewBag.VidFrame = codeAtWorkAppBL.PlayVideoById(playById);
            return View();
        }
    }
}