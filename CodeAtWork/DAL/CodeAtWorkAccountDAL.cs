using CodeAtWork.BL;
using CodeAtWork.Models.Session;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkAccountDAL : CommonDAL
    {
        internal FullUserDetail GetUserDetails(int userId)
        {
            FullUserDetail detail = new FullUserDetail();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@"  
                select * from AppUser AU
                inner join UserDetail UD on UD.AppUserId = AU.AppUserId
                where AU.AppUserId = {userId}
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                detail.FirstName = dataReader.GetValue(dataReader.GetOrdinal("FirstName")).ToString();
                detail.LastName = dataReader.GetValue(dataReader.GetOrdinal("LastName")).ToString();
                detail.Email = dataReader.GetValue(dataReader.GetOrdinal("Email")).ToString();
                detail.Username = dataReader.GetValue(dataReader.GetOrdinal("Username")).ToString();
                detail.AppUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")).ToString());
                detail.UserDetailId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserDetailId")).ToString());
                detail.Title = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Title")).ToString());
                detail.Company = dataReader.GetValue(dataReader.GetOrdinal("Company")).ToString();
                detail.YrsOfXP = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("XP")).ToString());
                detail.Role = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("Role")).ToString());
                detail.OrgLevel = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("LevelWithinOrg")).ToString());
            }

            dataReader.Close();
            command.Dispose();
            return detail;
        }

        internal void UpdateInfo(int userId, FullUserDetail updatedInfo)
        {
            SqlCommand command;

            string sql = $@"Update UserDetail Set FirstName = '{updatedInfo.FirstName}', LastName = '{updatedInfo.LastName}', Email = '{updatedInfo.Email}', Title = {updatedInfo.Title}, Company ='{updatedInfo.Company}'
                , XP = {updatedInfo.YrsOfXP}, role = {updatedInfo.Role}, LevelWithinOrg = {updatedInfo.OrgLevel} Where AppUserId = {userId};
            ";

            if(!string.IsNullOrEmpty(updatedInfo.Password))
            {
                sql += $@"  Update AppUser Set Password = '{EncryptionHelper.Encrypt(updatedInfo.Password)}' where AppUserId = {userId};";
            }

            command = new SqlCommand(sql, conn);
            command.ExecuteScalar();
        }

        internal bool VerifyPassword(string pwd, int userId)
        {
            bool validLogin = false;
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@"SELECT Password FROM AppUser where appuserid = {userId}";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (EncryptionHelper.Decrypt(dataReader.GetValue(dataReader.GetOrdinal("Password")).ToString()) == pwd)
                {
                    validLogin = true;
                    break;
                }
            }

            dataReader.Close();
            command.Dispose();
            return validLogin;
        }

        public Dictionary<string, int> GetAccountCounts(int userId)
        {
            Dictionary<string, int> accountCounts = new Dictionary<string, int>();
            SqlCommand command;
            SqlDataReader dataReader;

            string sql = $@"  
                select ( select count(1) from UserChannel
                where Appuserid = {userId}) as ChannelCount,
                (
                select  count(1) from path
                where pathid in (
                    select pathid from channelpath
                    inner join UserChannel on channelPath.UserChannelId = UserChannel.UserChannelId AND UserChannel.AppUserId = {userId}
                    )) as PathCount,
	                (

                Select count (1) from UserSubscribedTopic
                where Appuserid = {userId}
                ) as InterestCount 
            ";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (dataReader.GetValue(dataReader.GetOrdinal("ChannelCount")) != DBNull.Value)
                {
                    accountCounts.Add("ChannelCount", Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("ChannelCount"))));
                }
                else
                {
                    accountCounts.Add("ChannelCount", 0);
                }

                if (dataReader.GetValue(dataReader.GetOrdinal("PathCount")) != DBNull.Value)
                {
                    accountCounts.Add("PathCount", Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("PathCount"))));
                }
                else
                {
                    accountCounts.Add("PathCount", 0);
                }

                if (dataReader.GetValue(dataReader.GetOrdinal("InterestCount")) != DBNull.Value)
                {
                    accountCounts.Add("InterestCount", Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("InterestCount"))));
                }
                else
                {
                    accountCounts.Add("InterestCount", 0);
                }
            }

            return accountCounts;
        }
    }
}