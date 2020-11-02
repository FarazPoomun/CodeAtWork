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


        public bool VerifyPwd(string pwd)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
           return codeAtWorkAccountBL.VerifyPassword(pwd, userInfo.UserId);
        }

        public void UpdateInfo(FullUserDetail UpdatedInfo)
        {
            var userInfo = Session["UserInfo"] as UserInfo;
            codeAtWorkAccountBL.UpdateInfo(userInfo.UserId, UpdatedInfo);
        }
    }
}