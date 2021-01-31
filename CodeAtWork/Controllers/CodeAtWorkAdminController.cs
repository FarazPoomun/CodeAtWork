using CodeAtWork.BL;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
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
            ViewBag.UsernameLists = GetUsernameList();

            return View();
        }

        private HtmlString GetUsernameList()
        {
            return bl.GetUsernameList();

        }

        public HtmlString GetUserList(bool IsActive, string filterBy = null)
        {
            return bl.GetUsers(IsActive, filterBy);
        }

        public ActionResult AddNewVideo()
        {
            ViewBag.TopicsOptions = bl.GetTopics();

            return View();
        }

        public ActionResult ManageActiveAccount(IEnumerable<int> userIds, bool toDeactive)
        {
            bl.DeactivateAccounts(userIds, toDeactive);

            return RedirectToAction("UserManagement");
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

        public void DownloadReport(IEnumerable<int> userIds, bool includeInProgress, bool includeCompleted)
        {
            bl.GetDetailsAndDownload(userIds, includeInProgress, includeCompleted);
        }
    }
}