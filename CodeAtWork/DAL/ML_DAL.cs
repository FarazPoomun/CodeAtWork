using CodeAtWork.Models.Misc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CodeAtWork.DAL
{
    public class ML_DAL : CommonDAL
    {
        public Dictionary<int, List<int>> GetMLVideoIdsWithTopics()
        {
            Dictionary<int, List<int>> videoIds = new Dictionary<int, List<int>>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@"select MLVideoID, InterestCategoryTopicId from VideoTopic vt
                            inner join VideoRepository vr on vt.VideoId =  vr.VideoId";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var vidId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("MLVideoID")));
                if (videoIds.TryGetValue(vidId, out var val))
                {
                    videoIds[vidId].Add(Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("InterestCategoryTopicId"))));
                }
                else
                {
                    videoIds.Add(vidId, new List<int>() { Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("InterestCategoryTopicId"))) });
                }
            }

            dataReader.Close();
            command.Dispose();
            return videoIds;
        }
        
        public List<MLConnectorInterest> GetUserTopics(int userId)
        {
            List<MLConnectorInterest> topics = new List<MLConnectorInterest>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@"select * from UserSubscribedTopic
              where AppUserId  = {userId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var topic = new MLConnectorInterest()
                {
                    InterestId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("InterestCategoryTopicId"))),
                };

                topics.Add(topic);
            }

            dataReader.Close();
            command.Dispose();
            return topics;
        }


        internal int GetMLVideoId(Guid videoId)
        {
            int MLVideoId = -1;
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@" Select MLVideoID from VideoRepository where videoId = '{videoId}';";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                MLVideoId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("MLVideoID")).ToString());
            }

            dataReader.Close();
            command.Dispose();
            return MLVideoId;
        }

    }
}