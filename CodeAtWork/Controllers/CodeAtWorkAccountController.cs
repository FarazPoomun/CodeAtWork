using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkAccountController : Controller
    {
        // GET: CodeAtWorkAccount
        public ActionResult ViewMyAccount()
        {
            return View();
        }
    }
}