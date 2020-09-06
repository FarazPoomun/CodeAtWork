using CodeAtWork.BL;
using CodeAtWork.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkController : Controller
    {

        InterestsBL bl => new InterestsBL();
        // GET: CodeAtWork
        public ActionResult Home()
        {
            return View();
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
            //return bl.GetTopicsByCategoryName(CatergoryName);
        }
    }
}