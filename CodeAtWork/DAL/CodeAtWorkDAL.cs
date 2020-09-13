using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkDAL : CommonDAL
    {
        public CodeAtWorkDAL() : base()
        {

        }

        internal Dictionary<string, int> GetCatergoryIdsByName(List<string> CategoryNames)
        {
            Dictionary<string, int> topics = new Dictionary<string, int>();
            SqlCommand command;
            SqlDataReader dataReader;

            string catergoryNames = string.Join(",", CategoryNames.Select(z => "'" + z + "'"));
            //TO-DO Accomodate for UserId
            string sql = $@"SELECT InterestCategoryId, Name FROM InterestCategory where Name in ({catergoryNames})
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (!topics.ContainsKey(dataReader.GetValue(1).ToString()))
                {
                    topics.Add(dataReader.GetValue(1).ToString(), Convert.ToInt32(dataReader.GetValue(0)));
                }
            }

            dataReader.Close();
            command.Dispose();
            return topics;
        }

        internal int ValidateLoginDetail(string loginId, string pwd)
        {
            int validLogin = -1;
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for Email
            string sql = $@"SELECT AppUserId, Password FROM AppUser where username = '{loginId}'";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if(dataReader.GetValue(dataReader.GetOrdinal("Password")).ToString() == pwd)
                {
                    validLogin =Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")));
                    break;
                }
            }

            dataReader.Close();
            command.Dispose();
            return validLogin;
        }

        public List<InterestCatergoryTopic> GetTopicsByCategoryName(List<string> CategoryNames)
        {
            List<InterestCatergoryTopic> topics = new List<InterestCatergoryTopic>();
            SqlCommand command;
            SqlDataReader dataReader;

            string catergoryNames = string.Join(",", CategoryNames.Select(z => "'" + z + "'"));

            //TO-DO Accomodate for UserId
            string sql = $@"SELECT IGT.*, UST.InterestCategoryTopicId as selectedTopic FROM  InterestCategoryTopic IGT
            Left join UserSubscribedTopic UST on IGT.InterestCategoryTopicId =  UST.InterestCategoryTopicId
            inner join interestCategory IG on IGT.InterestCategoryId = IG.InterestCategoryId AND IG.Name in ({catergoryNames})
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var topic = new InterestCatergoryTopic()
                {
                    InterestCategoryTopicId = Convert.ToInt32(dataReader.GetValue(0)),
                    InterestCategoryId = Convert.ToInt32(dataReader.GetValue(1)),
                    Name = dataReader.GetValue(2).ToString()
                };

                topic.IsSelected = dataReader.GetValue(3) != DBNull.Value;
                topics.Add(topic);
            }

            dataReader.Close();
            command.Dispose();
            return topics;
        }

        public void SaveTopics(Dictionary<int, bool> updatedVal)
        {

            //TO-DO UserID in delete and insert
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();
            if (updatedVal.Values.Any(z => !z))
            {
                var toBeDeleted = updatedVal.Where(z => !z.Value).Select(z => z.Key).ToList();

                string sql = $"DELETE FROM UserSubscribedTopic WHERE InterestCategoryTopicId in ({string.Join(",", toBeDeleted)})";

                command = new SqlCommand(sql, conn);

                adapter.DeleteCommand = new SqlCommand(sql, conn);
                adapter.DeleteCommand.ExecuteNonQuery();
                command.Dispose();
            }

            if (updatedVal.Values.Any(z => z))
            {
                var toBeInserted = updatedVal.Where(z => z.Value).Select(z => z.Key).ToList();

                var values = toBeInserted.Select(z => "(1," + z + ")");
                string sql = $@"Insert into UserSubscribedTopic Values {string.Join(",", values)}";

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }

            adapter.Dispose();
        }

    }
}