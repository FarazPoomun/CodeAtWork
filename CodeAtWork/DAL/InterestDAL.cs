using CodeAtWork.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CodeAtWork.DAL
{
    public class InterestDAL : Common
    {
        public InterestDAL() : base()
        {

        }

        public List<InterestCatergoryTopic> GetTopicsByCategoryName(string CategoryName)
        {
            List<InterestCatergoryTopic> topics = new List<InterestCatergoryTopic>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@"SELECT IGT.*, UST.InterestCategoryTopicId as selectedTopic FROM  InterestCategoryTopic IGT
            Left join UserSubscribedTopic UST on IGT.InterestCategoryTopicId =  UST.InterestCategoryTopicId
            inner join interestCategory IG on IGT.InterestCategoryId = IG.InterestCategoryId AND IG.Name = '{CategoryName}'
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
    }
}