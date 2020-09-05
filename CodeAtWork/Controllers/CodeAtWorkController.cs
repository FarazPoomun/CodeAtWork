using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.Controllers
{
    public class CodeAtWorkController : Controller
    {
        // GET: CodeAtWork
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Interests()
        {
            return View();
        }
    }
}