using CodeAtWork.Models;
using CodeAtWork.Models.Misc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkAppDAL : CommonDAL
    {
        public CodeAtWorkAppDAL() : base()
        {

        }

        public List<VideoRepository> PlayVideoById(Guid vidId)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@" SELECT * FROM VideoRepository where VideoId = '{vidId}'
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString())
                };

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        internal List<VideoRepository> GetChannelVideos(int channelId)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   SELECT vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                                left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId
                                inner join ChannelVideo CV on CV.VideoId =  vr.VideoId AND CV.UserChannelId = {channelId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString())
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("UserBookMarkedVideo")) != DBNull.Value)
                {
                    vid.IsBookMarked = true;
                }

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        internal int GetDurationForWeek(int userId, DateTime thisWeekStart)
        {
            List<float> durations = new List<float>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@"
            ;with logCte as(
            select * ,
            Row_Number() over (partition by videoId Order by LastPlayedDuration) as DurationRank
            from UserVideoLog
            where LastModifiedTimestamp between '{thisWeekStart.ToString()}' AND CURRENT_TIMESTAMP AND AppUserId = {userId}
            )

            select lc.LastPlayedDuration from logCte lc
            where DurationRank = (select Max(DurationRank) from logCte where VideoId = lc.VideoId)";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                durations.Add(float.Parse(dataReader.GetValue(dataReader.GetOrdinal("LastPlayedDuration")).ToString()));
            }
            var duration = durations.Count > 0 ? durations.Sum(d => Convert.ToInt32(d / 60)) : 0;
            duration = duration > 60 ? 60 : duration;

            dataReader.Close();
            command.Dispose();
            return duration;
        }
        internal void CaptureTime(Guid videoId, float time, int IsFinished, int userId)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
               Insert into UserVideoLog
                Values('{videoId}', {userId}, {time}, default ,{IsFinished})";

            adapter.InsertCommand = new SqlCommand(sql, conn);
            adapter.InsertCommand.ExecuteNonQuery();
            adapter.Dispose();
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

        internal List<SubscribedChannelUser> GetSubscribeUserToChannel(int channelId)
        {
            List<SubscribedChannelUser> subscribedChannelUsers = new List<SubscribedChannelUser>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId
            string sql = $@" SELECT * FROM ChannelSubscribedUser where UserChannelId = {channelId};";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                SubscribedChannelUser subscribedChannelUser = new SubscribedChannelUser()
                {
                    ChannelSubscribedUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("ChannelSubscribedUserId")).ToString()),
                    UserChannelId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserChannelId")).ToString()),
                    Email = dataReader.GetValue(dataReader.GetOrdinal("Email")).ToString()
                };

                subscribedChannelUsers.Add(subscribedChannelUser);
            }

            dataReader.Close();
            command.Dispose();
            return subscribedChannelUsers;
        }

        internal void UnsubscribeUserToChannel(int channelSubscribedUserId)
        {
            SqlCommand command;
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $"DELETE FROM channelSubscribedUser WHERE channelSubscribedUserId ={channelSubscribedUserId}";

            command = new SqlCommand(sql, conn);

            adapter.DeleteCommand = new SqlCommand(sql, conn);
            adapter.DeleteCommand.ExecuteNonQuery();
            command.Dispose();
        }

        internal int? SubscribeUserToChannel(int channelId, string email)
        {
            SqlCommand command;
            SqlDataReader dataReader;
            bool validUser = false;
            string sql = $@"select 1 from UserDetail where email ='{email}'";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                validUser = true;
                break;
            }

            if (!validUser)
            {
                return -1;
            }

            dataReader.Close();
            command.Dispose();

            //TO-DO Accomodate for UserId And Interests
            sql = $@"select 1 from ChannelSubscribedUser where email ='{email}' and UserChannelId = {channelId}";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                return null;
            }

            dataReader.Close();
            command.Dispose();
            SqlDataAdapter adapter = new SqlDataAdapter();

            sql = $@"DECLARE @INSERTED table([ChannelSubscribedUserId] int);
            Insert into ChannelSubscribedUser
            OUTPUT INSERTED.[ChannelSubscribedUserId] Into @inserted
            values({ channelId},'{email}'); select* from @inserted";

            adapter.InsertCommand = new SqlCommand(sql, conn);

            var newId = (int)adapter.InsertCommand.ExecuteScalar();
            command.Dispose();
            adapter.Dispose();
            return newId;
        }

        internal List<UserChannel> GetPathChannels(int pathId, int userId)
        {
            List<UserChannel> vidChannelDetails = new List<UserChannel>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   select UC.*, CP.UserChannelId as IsSelected
                                from  UserChannel UC
                                left join channelPath CP on CP.UserChannelId = UC.UserChannelId AND pathId = {pathId}
                                where AppUserId  = {userId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                UserChannel UC = new UserChannel()
                {
                    UserChannelId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserChannelId")).ToString()),
                    ChannelName = dataReader.GetValue(dataReader.GetOrdinal("ChannelName")).ToString(),
                    IsShared = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsShared")).ToString()),
                    AppUserId = userId
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("IsSelected")) != DBNull.Value)
                {
                    UC.IsSelectedForVid = true;
                }
                vidChannelDetails.Add(UC);
            }

            dataReader.Close();
            command.Dispose();
            return vidChannelDetails;
        }

        public List<InterestCatergoryTopic> GetTopicsByCategoryName(List<string> CategoryNames, int userId)
        {
            List<InterestCatergoryTopic> topics = new List<InterestCatergoryTopic>();
            SqlCommand command;
            SqlDataReader dataReader;

            string catergoryNames = string.Join(",", CategoryNames.Select(z => "'" + z + "'"));

            //TO-DO Accomodate for UserId
            string sql = $@"SELECT IGT.*, UST.InterestCategoryTopicId as selectedTopic FROM  InterestCategoryTopic IGT
            Left join UserSubscribedTopic UST on IGT.InterestCategoryTopicId =  UST.InterestCategoryTopicId And UST.AppUserId = {userId}
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

        internal void AddAndLinkChannelToPath(int pathId, int userId, string channelName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
                DECLARE @INSERTED table ([UserChannelId] int);
                Insert into UserChannel 
                OUTPUT INSERTED.[UserChannelId] Into @inserted
                Values ('{channelName}', {userId}, default); select * from @inserted;";

            adapter.InsertCommand = new SqlCommand(sql, conn);

            var newId = (int)adapter.InsertCommand.ExecuteScalar();

            sql = $"Insert into ChannelPath Values ({newId}, {pathId});";
            adapter.InsertCommand = new SqlCommand(sql, conn);
            adapter.InsertCommand.ExecuteNonQuery();
            adapter.Dispose();
        }

        internal void AddPathToChannel(int channelId, int pathId)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
              	if(not exists(select 1 from channelpath where UserChannelId = {channelId} and pathid = {pathId}))
								Begin
								insert into channelpath values ({channelId}, {pathId})
								End";

            adapter.InsertCommand = new SqlCommand(sql, conn);
            adapter.InsertCommand.ExecuteNonQuery();
            adapter.Dispose();
        }

        public void SaveTopics(Dictionary<int, bool> updatedVal, int userId)
        {
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

                var values = toBeInserted.Select(z => $"({userId}," + z + ")");
                string sql = $@"Insert into UserSubscribedTopic Values {string.Join(",", values)}";

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }

            adapter.Dispose();
        }

        internal VideoWithTime GetVideoInfo(Guid vidId)
        {
            VideoWithTime vidDetails = new VideoWithTime();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@" 
            ;with vid_cte as(
            select vr.*, uvl.LastPlayedDuration,
            Row_Number() over (partition by uvl.VideoId order by LastPlayedDuration) as logOrder
            from videorepository vr
            left join UserVideoLog uvl on uvl.VideoId = vr.VideoId
            where vr.VideoId = '{vidId}'
            )

            select * from vid_cte
            where logOrder = (select max(logOrder) from vid_cte)";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                vidDetails = new VideoWithTime()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString())
                };

                vidDetails.SeekTo = dataReader.GetValue(dataReader.GetOrdinal("LastPlayedDuration")) != DBNull.Value ?
              float.Parse(dataReader.GetValue(dataReader.GetOrdinal("LastPlayedDuration")).ToString()) : 1;
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        internal void UpdateIsShared(int userChannelId, bool IsShared)
        {
            SqlCommand command;

            int isShared = IsShared ? 1 : 0;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"  Update UserChannel Set IsShared = {isShared} where UserChannelId = {userChannelId} 
            ";

            command = new SqlCommand(sql, conn);
            command.ExecuteScalar();
        }

        internal ChannelHeaderInfo GetChannelInfo(int channelId)
        {
            ChannelHeaderInfo channelHeaderInfo = new ChannelHeaderInfo();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"  select * from userchannel UC 
                            Inner join UserDetail AU on AU.AppUserId = UC.AppUserId
                            where uc.UserChannelId = {channelId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                channelHeaderInfo.UserChannelId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserChannelId")));
                channelHeaderInfo.CreatedBy = $"{dataReader.GetValue(dataReader.GetOrdinal("FirstName")).ToString()} {dataReader.GetValue(dataReader.GetOrdinal("LastName")).ToString()}";
                channelHeaderInfo.ChannelName = dataReader.GetValue(dataReader.GetOrdinal("ChannelName")).ToString();

                if (dataReader.GetValue(dataReader.GetOrdinal("IsShared")) != DBNull.Value)
                {
                    channelHeaderInfo.IsShared = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsShared")));
                }
            }

            dataReader.Close();
            command.Dispose();
            return channelHeaderInfo;
        }

        internal List<UserChannel> GetVideoChannels(Guid vidId, int userId)
        {
            List<UserChannel> vidChannelDetails = new List<UserChannel>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   select UC.*, CV.UserChannelId as IsSelected
                                from  UserChannel UC
                                left join channelVideo CV  on CV.UserChannelId = UC.UserChannelId AND VideoId = '{vidId}'
                                where AppUserId  = {userId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                UserChannel UC = new UserChannel()
                {
                    UserChannelId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserChannelId")).ToString()),
                    ChannelName = dataReader.GetValue(dataReader.GetOrdinal("ChannelName")).ToString(),
                    IsShared = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsShared")).ToString()),
                    AppUserId = userId
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("IsSelected")) != DBNull.Value)
                {
                    UC.IsSelectedForVid = true;
                }
                vidChannelDetails.Add(UC);
            }

            dataReader.Close();
            command.Dispose();
            return vidChannelDetails;

        }

        internal void DeleteChannels(List<int> channelIdsToDelete)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command;
            string sql = $"DELETE FROM UserChannel WHERE UserChannelId in ({string.Join(",", channelIdsToDelete)})";

            command = new SqlCommand(sql, conn);

            adapter.DeleteCommand = new SqlCommand(sql, conn);
            adapter.DeleteCommand.ExecuteNonQuery();
            command.Dispose();

            adapter.Dispose();
        }

        internal void AddOrRemovePathChannelFromVid(int pathId, int channelId, bool isSelected, int userId)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            if (isSelected)
            {
                SqlDataReader dataReader;
                SqlCommand command;

                string sql = $@"select 1 asAlreadySelected from ChannelPath where  userchannelId = {channelId} and pathid = {pathId}";

                command = new SqlCommand(sql, conn);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.GetValue(dataReader.GetOrdinal("asAlreadySelected")) != DBNull.Value)
                    {
                        return;
                    }
                }

                dataReader.Close();
                command.Dispose();

                sql = $@"Insert into ChannelPath Values ({channelId}, {pathId})";

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command;
                string sql = $"DELETE FROM ChannelPath WHERE pathid = {pathId} and UserChannelId = {channelId}";

                command = new SqlCommand(sql, conn);

                adapter.DeleteCommand = new SqlCommand(sql, conn);
                adapter.DeleteCommand.ExecuteNonQuery();
                command.Dispose();
            }

            adapter.Dispose();
        }

        internal PathDetail GetPathDetail(int pathId)
        {
            PathDetail detail = new PathDetail() { PathId = pathId };
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlDataReader dataReader;
            SqlCommand command;

            string sql = $@"select p.pathId, Name, Description from Path p inner join PathDetail pd on p.PathId = pd.PathId
                         where p.pathid = {pathId}";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                detail.Name = dataReader.GetValue(dataReader.GetOrdinal("Name")).ToString();
                detail.Description = dataReader.GetValue(dataReader.GetOrdinal("Description")).ToString();
            }
            dataReader.Close();
            sql = $"select * from PathPrerequisite where pathid = {pathId} ";
            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                detail.Prerequisites.Add(dataReader.GetValue(dataReader.GetOrdinal("Prerequisite")).ToString());
            }
            dataReader.Close();

            sql = $"select * from PathOutcome where pathid = {pathId} ";
            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                detail.Outcomes.Add(dataReader.GetValue(dataReader.GetOrdinal("Outcome")).ToString());
            }

            dataReader.Close();
            command.Dispose();

            return detail;
        }

        internal void AddOrRemoveChannelFromVid(Guid videoId, int channelId, bool isSelected, int userId)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            if (isSelected)
            {
                SqlDataReader dataReader;
                SqlCommand command;

                string sql = $@"select 1 asAlreadySelected from ChannelVideo where  userchannelId = {channelId} and VideoId = '{videoId}'";

                command = new SqlCommand(sql, conn);
                dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {


                    if (dataReader.GetValue(dataReader.GetOrdinal("asAlreadySelected")) != DBNull.Value)
                    {
                        return;
                    }
                }

                dataReader.Close();
                command.Dispose();

                sql = $@"Insert into ChannelVideo Values ({channelId}, '{videoId}')";

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command;
                string sql = $"DELETE FROM ChannelVideo WHERE VideoId = '{videoId}' and UserChannelId = {channelId}";

                command = new SqlCommand(sql, conn);

                adapter.DeleteCommand = new SqlCommand(sql, conn);
                adapter.DeleteCommand.ExecuteNonQuery();
                command.Dispose();
            }

            adapter.Dispose();
        }

        internal List<VideoRepository> SearchVid(string searchedTxt)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   SELECT vr.* FROM VideoRepository vr
                                WHERE vr.VideoDescription like'%{searchedTxt}%'
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString())
                };

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        internal void SaveNewBookMark(Guid videoId, int userId, bool isSelected)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            if (isSelected)
            {
                string sql = $@"Insert into UserBookMarkedVideo Values ('{videoId}', {userId}, default)";

                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            else
            {
                SqlCommand command;
                string sql = $"DELETE FROM UserBookMarkedVideo WHERE VideoId = '{videoId}' and AppUserId = {userId}";

                command = new SqlCommand(sql, conn);

                adapter.DeleteCommand = new SqlCommand(sql, conn);
                adapter.DeleteCommand.ExecuteNonQuery();
                command.Dispose();
            }

            adapter.Dispose();
        }

        internal void AddAndLinkChannel(Guid? videoId, int userId, string channelName)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
                DECLARE @INSERTED table ([UserChannelId] int);
                Insert into UserChannel 
                OUTPUT INSERTED.[UserChannelId] Into @inserted
                Values ('{channelName}', {userId}, default); select * from @inserted;";

            adapter.InsertCommand = new SqlCommand(sql, conn);

            if (videoId is null)
            {
                adapter.InsertCommand.ExecuteNonQuery();
            }
            else
            {
                var newId = (int)adapter.InsertCommand.ExecuteScalar();

                sql = $"Insert into ChannelVideo Values ({newId}, '{videoId}')";
                adapter.InsertCommand = new SqlCommand(sql, conn);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            adapter.Dispose();
        }

        public List<VideoRepository> GetRecentViewedVids(int userId, int? top = 5)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "";
            if (top is null)
            {
                //TO-DO Accomodate for UserId And Interests
                sql = $@"
                    ;with logCte as(
                    select distinct(uvl.VideoId)
                    from UserVideoLog uvl
                    where AppUserId = {userId} and not exists (select 1 from UserVideoLog where IsFinished = 1 and VideoId = uvl.VideoId)
                    )

                     SELECT vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                                            left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
						                    where Vr.VideoId in (select * from logCte)
            ";
            }
            else
            {
                //TO-DO Accomodate for UserId And Interests
                sql = $@"     ;with logCte as(
                    select distinct(uvl.VideoId)
                    from UserVideoLog uvl
                    where AppUserId = {userId} and not exists (select 1 from UserVideoLog where IsFinished = 1 and VideoId = uvl.VideoId)
                    )

                     SELECT top {top} vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                                            left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
						                    where Vr.VideoId in (select * from logCte)
            ";
            }

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString()),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString())
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("UserBookMarkedVideo")) != DBNull.Value)
                {
                    vid.IsBookMarked = true;
                }

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }
        
        public List<VideoRepository> GetRecommendedVids(int userId, int? top = 5)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;
            string sql = "";
            if (top is null)
            {
                //TO-DO Accomodate for UserId And Interests
                sql = $@" SELECT  vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                        left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
            ";
            }
            else
            {
                //TO-DO Accomodate for UserId And Interests
                sql = $@" SELECT top {top} vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                        left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
            ";
            }

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString()),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString())
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("UserBookMarkedVideo")) != DBNull.Value)
                {
                    vid.IsBookMarked = true;
                }

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        public List<VideoRepository> GetBookmarkedVideos(int userId)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   SELECT vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                        inner join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                VideoRepository vid = new VideoRepository()
                {
                    VideoId = Guid.Parse(dataReader.GetValue(dataReader.GetOrdinal("VideoId")).ToString()),
                    VideoAuthor = dataReader.GetValue(dataReader.GetOrdinal("VideoAuthor")).ToString(),
                    VideoURL = dataReader.GetValue(dataReader.GetOrdinal("VideoURL")).ToString(),
                    VideoDescription = dataReader.GetValue(dataReader.GetOrdinal("VideoDescription")).ToString(),
                    IsLocal = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsLocal")).ToString()),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString()),
                };

                if (dataReader.GetValue(dataReader.GetOrdinal("UserBookMarkedVideo")) != DBNull.Value)
                {
                    vid.IsBookMarked = true;
                }

                vidDetails.Add(vid);
            }

            dataReader.Close();
            command.Dispose();
            return vidDetails;
        }

        public List<UserChannelWithCounts> GetChannelLists(int userId, int isShared)
        {
            List<UserChannelWithCounts> channelDetails = new List<UserChannelWithCounts>();
            SqlCommand command;
            SqlDataReader dataReader;

            string additionalJoin = "";
            string additionalWhere = "";

            if (isShared == 1)
            {
                additionalJoin = "left join ChannelSubscribedUser csu on csu.UserChannelId = UC.UserChannelId";
                additionalWhere = "OR UD.Email =csu.Email";
            }

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   select UC.*, 
                                (select count(*)from ChannelVideo
                                where ChannelVideo.UserChannelId = UC.UserChannelId ) as VideoCount,
                                (select count(*)from ChannelPath
                                where ChannelPath.UserChannelId = UC.UserChannelId ) as PathCount,
								CONCAT(UD.FirstName, ' ', UD.Lastname) as CreatedBy
                                from  UserChannel UC
								inner join UserDetail UD on UD.AppUserId = {userId}
                                {additionalJoin}
                                where (UC.AppUserId  = {userId} {additionalWhere}) And IsShared  = {isShared}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                UserChannelWithCounts UC = new UserChannelWithCounts()
                {
                    UserChannelId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserChannelId")).ToString()),
                    ChannelName = dataReader.GetValue(dataReader.GetOrdinal("ChannelName")).ToString(),
                    IsShared = Convert.ToBoolean(dataReader.GetValue(dataReader.GetOrdinal("IsShared")).ToString()),
                    VideoCount = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("VideoCount")).ToString()),
                    PathCount = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("PathCount")).ToString()),
                    CreatedBy = dataReader.GetValue(dataReader.GetOrdinal("CreatedBy")).ToString(),
                    AppUserId = userId
                };
                if (!channelDetails.Any(z => z.UserChannelId == UC.UserChannelId))
                {
                    channelDetails.Add(UC);
                }
            }

            dataReader.Close();
            command.Dispose();
            return channelDetails;
        }

        internal List<Path> GetAllPaths(int userId, CategoryEnum? category, int tabId)
        {
            List<Path> paths = new List<Path>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@" select * from path ";

            if (tabId == 2) //2 == Following
            {
                sql += $@"where pathid in (
                        select pathid from channelpath
                        inner join UserChannel on channelPath.UserChannelId = UserChannel.UserChannelId AND UserChannel.AppUserId = {userId}
                        )";
            }

            if (category != null)
            {
                if (tabId == 2)
                {
                    sql += " And category = {(int)category}";
                }
                else
                    sql += $" where category = {(int)category}";
            }

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Path path = new Path()
                {
                    PathId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("PathId")).ToString()),
                    Name = dataReader.GetValue(dataReader.GetOrdinal("Name")).ToString(),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString()),
                };
                paths.Add(path);
            }
            dataReader.Close();
            command.Dispose();
            return paths;
        }

        internal List<Path> GetAllPathsFiltered(string filterBy)
        {
            List<Path> paths = new List<Path>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@" select * from path where Name like '%{filterBy}%'";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Path path = new Path()
                {
                    PathId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("PathId")).ToString()),
                    Name = dataReader.GetValue(dataReader.GetOrdinal("Name")).ToString(),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString()),
                };
                paths.Add(path);
            }
            dataReader.Close();
            command.Dispose();
            return paths;
        }

        internal List<Path> GetPathsPanePerChannelId(int channelId)
        {
            List<Path> paths = new List<Path>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@" select * from path 
                            inner join channelPath cp on path.pathId = cp.pathId AND cp.UserChannelId = {channelId}";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                Path path = new Path()
                {
                    PathId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("PathId")).ToString()),
                    Name = dataReader.GetValue(dataReader.GetOrdinal("Name")).ToString(),
                    Level = (LevelsEnum)Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Level")).ToString()),
                };
                paths.Add(path);
            }
            dataReader.Close();
            command.Dispose();
            return paths;
        }
    }
}