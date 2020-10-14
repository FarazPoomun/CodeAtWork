using CodeAtWork.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class CodeAtWorkAccountBL
    {
        CodeAtWorkAccountDAL dal;

        public CodeAtWorkAccountBL()
        {
            dal = new CodeAtWorkAccountDAL();
        }

        public void GetUserDetails(int userId)
        {
            dal.GetUserDetails(userId);
        }
    }
}