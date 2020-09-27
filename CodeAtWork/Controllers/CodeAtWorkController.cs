﻿using CodeAtWork.BL;
using CodeAtWork.Models;
using CodeAtWork.Models.Misc;
using CodeAtWork.Models.Session;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public ActionResult Register()
        {
            return View();
        }

        public JsonResult SaveRegistration(UserDetails user)
        {
            RegistrationValidations result = new RegistrationValidations();

            var foo = new EmailAddressAttribute();

            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                result.HasValidationFailed = true;
                result.ValidationMsg = "Invalid Email Format.";
                return Json(result);
            }
            else if (bl.ValidateUsername(user.Username))
            {
                result.HasValidationFailed = true;
                result.ValidationMsg = "Please Choose A Different Username.";
                return Json(result);
            }
            else
            {
                user.Password = EncryptionHelper.Encrypt(user.Password);
                var userId = bl.SaveRegistration(user);
                UserInfo newUser = new UserInfo()
                {
                    UserId = userId,
                    FirstName = user.FirstName

                };
                Session["UserInfo"] = newUser;
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult ValidateLoginInfo()
        {
            var loginId = Request["LoginId"];
            var password =Request["Password"];
            var validLogin = bl.ValidateLoginDetail(loginId, password);

            if (validLogin > 0)
            {
                Session["UserInfo"] = new UserInfo()
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