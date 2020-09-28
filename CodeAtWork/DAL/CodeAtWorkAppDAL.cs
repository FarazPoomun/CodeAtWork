﻿using CodeAtWork.Models;
using CodeAtWork.Models.Misc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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

        internal void AddOrRemoveChannelFromVid(Guid videoId, int channelId, bool isSelected, int userId)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            if (isSelected)
            {
                string sql = $@"Insert into ChannelVideo Values ({channelId}, '{videoId}')";

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

            if(videoId is null)
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


        public List<VideoRepository> GetRecommendedVids(int userId)
        {
            List<VideoRepository> vidDetails = new List<VideoRepository>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   SELECT vr.*, UBV.UserBookMarkedVideo FROM VideoRepository vr
                        left join Userbookmarkedvideo UBV on vr.VideoId =  UBV.VideoId AND UBV.AppUserId = {userId}
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


        public List<UserChannelWithCounts> GetChannelLists(int userId)
        {
            List<UserChannelWithCounts> channelDetails = new List<UserChannelWithCounts>();
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for UserId And Interests
            string sql = $@"   select UC.*, 
                                (select count(*)from ChannelVideo
                                where ChannelVideo.UserChannelId = UC.UserChannelId ) as VideoCount,
								CONCAT(UD.FirstName, ' ', UD.Lastname) as CreatedBy
                                from  UserChannel UC
								inner join UserDetail UD on UD.AppUserId = {userId}
                                where UC.AppUserId  = {userId} And IsShared  = 0
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
                    CreatedBy = dataReader.GetValue(dataReader.GetOrdinal("CreatedBy")).ToString(),
                    AppUserId = userId
                };
                channelDetails.Add(UC);
            }

            dataReader.Close();
            command.Dispose();
            return channelDetails;
        }

    }
}