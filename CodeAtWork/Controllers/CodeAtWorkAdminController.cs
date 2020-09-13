using CodeAtWork.BL.Admin;
using CodeAtWork.Models.UI;
using System;
using System.IO;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAdminController : Controller
    {
        AdminBL bl => new AdminBL();
        // GET: CodeAtWorkAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddNewVideo() => View();
        public ActionResult LinkNewVideo() => View();


        [HttpPost]
        public void SaveVid()
        {
            var newVid = new CreateVid()
            {
                ImageFile = Request.Files[0],
                VideoURL = Request["VideoURL"],
                IsLocal = Convert.ToBoolean(Request["isLocal"]),
                VideoAuthor = Request["VideoAuthor"],
                VideoDescription = Request["VideoDescription"]
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