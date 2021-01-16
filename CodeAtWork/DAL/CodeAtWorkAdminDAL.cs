using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using CodeAtWork.Models.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkAdminDAL : CommonDAL
    {
        public List<InterestCatergoryTopic> GetTopics()
        {
            List<InterestCatergoryTopic> topics = new List<InterestCatergoryTopic>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@"select * from  InterestCategoryTopic
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var topic = new InterestCatergoryTopic()
                {
                    InterestCategoryTopicId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("InterestCategoryTopicId"))),
                    Name = dataReader.GetValue(dataReader.GetOrdinal("Name")).ToString()
                };

                topics.Add(topic);
            }

            dataReader.Close();
            command.Dispose();
            return topics;
        }

        internal List<FullUserDetail> GetUsersList(bool isActive)
        {
            List<FullUserDetail> detail = new List<FullUserDetail>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = @"  
                select * from AppUser AU
                inner join UserDetail UD on UD.AppUserId = AU.AppUserId
            ";

            if (isActive)
            {
                sql += @"
                left join UserActiveHistory uah on uah.AppUserId = AU.AppUserId AND
                (uah.IsActive is null or uah.IsActive = 1)";
            }
            else{
                sql += @" 
                inner join UserActiveHistory uah on uah.AppUserId = AU.AppUserId AND uah.IsActive = 0";
            }

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                var oneUserDetail = new FullUserDetail
                {
                    FirstName = dataReader.GetValue(dataReader.GetOrdinal("FirstName")).ToString(),
                    LastName = dataReader.GetValue(dataReader.GetOrdinal("LastName")).ToString(),
                    Email = dataReader.GetValue(dataReader.GetOrdinal("Email")).ToString(),
                    Username = dataReader.GetValue(dataReader.GetOrdinal("Username")).ToString(),
                    AppUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")).ToString()),
                    UserDetailId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserDetailId")).ToString())
                };
                detail.Add(oneUserDetail);
            }

            dataReader.Close();
            command.Dispose();
            return detail;
        }

        internal List<UserChannelWithCounts> GetUsersList(object userId, bool isActive)
        {
            throw new NotImplementedException();
        }

        public Guid SaveNewVideo(CreateVid vid)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
                DECLARE @INSERTED table ([VideoId] uniqueIdentifier);
                Insert into VideoRepository 
                OUTPUT INSERTED.[VideoId] Into @inserted
                Values (default, '{vid.VideoURL}', '{vid.IsLocal}','{vid.VideoAuthor}','{vid.VideoDescription}', default); select * from @inserted;";

            adapter.InsertCommand = new SqlCommand(sql, conn);
            var newId = (Guid)adapter.InsertCommand.ExecuteScalar();

            var relatedTopics = vid.RelatedTopicIds.Split(',');
            if (relatedTopics.Length > 0)
            {
                sql = "Insert into VideoTopic Values";
                for (int i = 0; i < relatedTopics.Length; i++)
                {
                    sql += $"( '{newId}', {relatedTopics[i]} )";

                    if (i + 1 < relatedTopics.Length)
                    {
                        sql += ",";
                    }
                }

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteScalar();
            }
            adapter.Dispose();
            return newId;
        }
    }
}