using CodeAtWork.DAL;
using CodeAtWork.Models.Session;
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

        public FullUserDetail GetUserDetails(int userId)
        {
          return dal.GetUserDetails(userId);
        }

        public Dictionary<string, int> GetAccountCounts(int userId)
        {
            return dal.GetAccountCounts(userId);
        }

        internal bool VerifyPassword(string pwd, int userId)
        {
            return dal.VerifyPassword(pwd, userId);
        }

        internal void UpdateInfo(int userId, FullUserDetail updatedInfo)
        {
             dal.UpdateInfo(userId, updatedInfo);
        }
    }
}