using CodeAtWork.BL;
using CodeAtWork.Models.UI;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAdminController : Controller
    {
        CodeAtWorkAdminBL bl => new CodeAtWorkAdminBL();
        // GET: CodeAtWorkAdmin
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult UserManagement()
        {
            ViewBag.UsersTabData = GetUserList(true);

            return View();
        }

        public HtmlString GetUserList(bool IsActive)
        {
            return bl.GetUsers(IsActive);
        }

        public ActionResult AddNewVideo()
        {
            ViewBag.TopicsOptions = bl.GetTopics();

            return View();
        }


        [HttpPost]
        public void SaveVid()
        {
            var newVid = new CreateVid()
            {
                ImageFile = Request.Files[0],
                VideoURL = Request["VideoURL"],
                IsLocal = Convert.ToBoolean(Request["isLocal"]),
                VideoAuthor = Request["VideoAuthor"],
                VideoDescription = Request["VideoDescription"],
                RelatedTopicIds = Request["RelatedTopics"]
            };

            CaptureNewVidSnapShot(newVid, bl.SaveNewVid(newVid));
        }

        public void CaptureNewVidSnapShot(CreateVid newVid, Guid newId)
        {
            var path = Path.Combine(Server.MapPath("~/Uploads/"), newId.ToString() + ".jpg");
            newVid.ImageFile.SaveAs(path);
        }
    }
}