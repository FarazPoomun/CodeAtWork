using CodeAtWork.DAL;
using CodeAtWork.Models.Session;

namespace CodeAtWork.BL
{
    public class CodeAtWorkBL
    {
        CodeAtWorkDAL dal;
        public CodeAtWorkBL()
        {
            dal = new CodeAtWorkDAL();
        }

        #region DB calls
        public int ValidateLoginDetail(string loginId, string pwd)
        {
            return dal.ValidateLoginDetail(loginId, pwd);
        }

        public bool ValidateUsername(string username)
        {
            return dal.ValidateUsername(username);
        }

        internal int SaveRegistration(UserDetails user)
        {
            return dal.SaveRegistration(user);
        }

        internal UserInfo GetUserInfo(int userId)
        {
            return dal.GetUserInfo(userId);
        }

        #endregion

    }
}