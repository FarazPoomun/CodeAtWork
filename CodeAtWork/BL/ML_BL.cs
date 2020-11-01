using CodeAtWork.DAL;
using CodeAtWork.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeAtWork.BL
{
    public class ML_BL
    {
       private ML_DAL dal;
        public ML_BL()
        {
            dal = new ML_DAL();
        }

        public Dictionary<int, List<int>> GetMLVideoIds()
        {
            return dal.GetMLVideoIdsWithTopics();
        }

        public List<MLConnectorInterest> GetUserTopics(int userId)
        {
            return dal.GetUserTopics(userId);
        }

        public int GetMLVideoId(Guid videoId)
        {
            return dal.GetMLVideoId(videoId);
        }
    }
}