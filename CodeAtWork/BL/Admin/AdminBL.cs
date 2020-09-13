using CodeAtWork.DAL.Admin;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeAtWork.BL.Admin
{
    public class AdminBL
    {
        AdminDAL dal;

        public AdminBL()
        {
            dal = new AdminDAL();
        }

        public Guid SaveNewVid(CreateVid vid)
        {
            return dal.SaveNewVideo(vid);
        }

    }
}