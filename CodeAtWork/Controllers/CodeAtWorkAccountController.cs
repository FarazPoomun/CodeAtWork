using CodeAtWork.BL;
using CodeAtWork.Models.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAccountController : Controller
    {
        readonly CodeAtWorkAccountBL codeAtWorkAccountBL = new CodeAtWorkAccountBL();

        // GET: CodeAtWorkAccount
        public ActionResult ViewMyAccount()
        {
            if (Session["UserInfo"] == null)
            {
                return RedirectToAction("Login", "CodeAtWork");
            }

            //GetRecommended
            var userInfo = Session["UserInfo"] as UserInfo;
            ViewBag.UserDetails = codeAtWorkAccountBL.GetUserDetails(userInfo.UserId);
            ViewBag.AccountCounts = codeAtWorkAccountBL.GetAccountCounts(userInfo.UserId);
            return View();
        }
    }
}