using CodeAtWork.BL;
using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkController : Controller
    {

        CodeAtWorkBL bl => new CodeAtWorkBL();
        // GET: CodeAtWork
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Login(bool invalidLogin = false)
        {
            ViewBag.InvalidLogin = invalidLogin;
            return View();
        }

        [HttpPost]
        public ActionResult ValidateLoginInfo()
        {
            var loginId = Request["LoginId"];
            var password = Request["Password"];
            var validLogin = bl.ValidateLoginDetail(loginId, password);

            if (validLogin > 0)
            {
                Session["UserInfo"] = new UserInfo ()
                {
                    UserId = validLogin
                };

               return RedirectToAction("Home", "CodeAtWorkApp");
            }
            else
            {

                return RedirectToAction("Login", new { invalidLogin = true });
            }
        }

        public ActionResult Interests()
        {
            ViewBag.TopicPills = bl.GetTopicsByCategoryName();
            return View();
        }

        public HtmlString GetTopicsByCategoryName(string CatergoryName)
        {
            return bl.GetTopicsByCategoryName(CatergoryName);
        }

        public void UpdateInterestTopics(List<InterestCategoryTopicToBeSaved> InterestTopics)
        {
            bl.SaveTopics(InterestTopics);
        }
    }
}