using CodeAtWork.BL;
using CodeAtWork.Models.Misc;
using CodeAtWork.Models.Session;
using System.ComponentModel.DataAnnotations;
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
            Models.Misc.ValidationResult result = new Models.Misc.ValidationResult();

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
            var password = Request["Password"];
            var validLogin = bl.ValidateLoginDetail(loginId, password);

            if (validLogin > 0)
            {
                Session["UserInfo"] = bl.GetUserInfo(validLogin);

                return RedirectToAction("Home", "CodeAtWorkApp");
            }
            else
            {
                return RedirectToAction("Login", new { invalidLogin = true });
            }
        }
    }
}