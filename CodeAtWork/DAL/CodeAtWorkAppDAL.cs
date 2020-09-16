﻿using CodeAtWork.Models;
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

    }
}