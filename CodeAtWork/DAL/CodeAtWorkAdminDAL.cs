using CodeAtWork.Models;
using CodeAtWork.Models.Session;
using CodeAtWork.Models.UI;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkAdminDAL : CommonDAL
    {
        public List<InterestCatergoryTopic> GetTopics()
        {
            List<InterestCatergoryTopic> topics = new List<InterestCatergoryTopic>();
            SqlCommand command;
            SqlDataReader dataReader;

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

        internal List<FullUserDetail> GetUsersList(IEnumerable<int> userIds = null)
        {
            List<FullUserDetail> AllUsersdetail = new List<FullUserDetail>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@"  
                select * from AppUser AU
                inner join UserDetail UD on UD.AppUserId = AU.AppUserId
            ";

            if (userIds != null)
            {
                sql += $"Where AU.AppUserId in({string.Join(",", userIds)})";
            }

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                FullUserDetail detail = new FullUserDetail
                {
                    FirstName = dataReader.GetValue(dataReader.GetOrdinal("FirstName")).ToString(),
                    LastName = dataReader.GetValue(dataReader.GetOrdinal("LastName")).ToString(),
                    Email = dataReader.GetValue(dataReader.GetOrdinal("Email")).ToString(),
                    Username = dataReader.GetValue(dataReader.GetOrdinal("Username")).ToString(),
                    AppUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")).ToString()),
                    UserDetailId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserDetailId")).ToString()),
                    Title = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Title")).ToString()),
                    Company = dataReader.GetValue(dataReader.GetOrdinal("Company")).ToString(),
                    YrsOfXP = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("XP")).ToString()),
                    Role = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Role")).ToString()),
                    OrgLevel = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("LevelWithinOrg")).ToString()),
                    CreatedOn = Convert.ToDateTime(dataReader.GetValue(dataReader.GetOrdinal("CreatedOn")).ToString()),
                    LastLogin = Convert.ToDateTime(dataReader.GetValue(dataReader.GetOrdinal("LastLogin")).ToString())
                };

                AllUsersdetail.Add(detail);
            }

            dataReader.Close();
            command.Dispose();
            return AllUsersdetail;
        }

        internal List<UserVideoLog> GetVideoLog(IEnumerable<int> userIds)
        {
            List<UserVideoLog> AllUsersdetail = new List<UserVideoLog>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@"  
                select * from UserVideoLog where AppUserId in ({string.Join(",", userIds)})
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                UserVideoLog detail = new UserVideoLog
                {
                    UserVideoLogId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserVideoLogId")).ToString()),
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    AppUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")).ToString()),
                    LastPlayedDuration = float.Parse(dataReader.GetValue(dataReader.GetOrdinal("LastPlayedDuration")).ToString()),
                    LastModifiedTimestamp = Convert.ToDateTime(dataReader.GetValue(dataReader.GetOrdinal("LastModifiedTimestamp")).ToString()),
                    IsFinished = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsFinished")).ToString())
                };

                AllUsersdetail.Add(detail);
            }

            dataReader.Close();
            command.Dispose();
            return AllUsersdetail;
        }

        internal List<VideoRepository> GetVideoIdAndName(List<Guid> videoIds)
        {
            var formattedIds = videoIds.Select(v => $"'{v}'");
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@" SELECT * FROM VideoRepository where VideoId in ({string.Join(",", formattedIds)})
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                };

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        internal void DeactivateAccounts(IEnumerable<int> userIds, bool toDeactive)
        {
            SqlCommand command;

            if (toDeactive)
            {
                SqlDataReader dataReader;
                SqlDataAdapter adapter = new SqlDataAdapter();
                userIds.ForEach(id =>
                {
                    string sqlSelect = $@"select * from  UserActiveHistory where AppUserId = {id}";
                    int? UserActiveHistoryId = null;
                    command = new SqlCommand(sqlSelect, conn);
                    dataReader = command.ExecuteReader();
                    if (dataReader.Read())
                    {
                        command.Dispose();
                        UserActiveHistoryId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserActiveHistoryId")));

                    }

                    if (UserActiveHistoryId != null)
                    {
                        dataReader.Close();
                        SqlCommand updateConn;

                        string sqlUpdate = $@"  Update UserActiveHistory Set IsActive = 0 where UserActiveHistoryId = { UserActiveHistoryId}";
                        updateConn = new SqlCommand(sqlUpdate, conn);
                        updateConn.ExecuteScalar();
                        updateConn.Dispose();
                    }

                    else
                    {
                        dataReader.Close();
                        command.Dispose();

                        string sql = $@"Insert into UserActiveHistory Values ({id}, 0)";

                        adapter.InsertCommand = new SqlCommand(sql, conn);
                        adapter.InsertCommand.ExecuteNonQuery();
                    }
                });
                adapter.Dispose();
            }

            else
            {
                string sqlUpdate = $@"  Update UserActiveHistory Set IsActive = 1 where AppUserId in ({string.Join(",", userIds)})";
                command = new SqlCommand(sqlUpdate, conn);
                command.ExecuteScalar();
            }
        }

        internal List<FullUserDetail> GetUsersList(bool isActive, string filterBy)
        {
            List<FullUserDetail> detail = new List<FullUserDetail>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = @"  
                select * from AppUser AU
                inner join UserDetail UD on UD.AppUserId = AU.AppUserId
            ";

            if (filterBy != null)
            {
                sql += $" And (UD.FirstName like '%{filterBy}%' OR UD.LastName like '%{filterBy}%') ";
            }

            if (isActive)
            {
                sql += @"
                left join UserActiveHistory uah on uah.AppUserId = AU.AppUserId
               where uah.IsActive is null or uah.IsActive = 1";
            }
            else
            {
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