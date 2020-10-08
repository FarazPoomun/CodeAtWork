using CodeAtWork.Models;
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

           var relatedTopics =  vid.RelatedTopicIds.Split(',');
            if (relatedTopics.Length > 0)
            {
                sql = "Insert into VideoTopic Values";
                for (int i = 0; i < relatedTopics.Length; i++)
                {
                    sql += $"( '{newId}', {relatedTopics[i]} )";

                    if (i+1 < relatedTopics.Length)
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