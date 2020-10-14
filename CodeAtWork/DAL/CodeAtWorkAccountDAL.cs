using CodeAtWork.Models.Session;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CodeAtWork.DAL
{
    public class CodeAtWorkAccountDAL : CommonDAL
    {
        internal UserDetailsWithId GetUserDetails(int userId)
        {
            UserDetailsWithId details = new UserDetailsWithId();
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
                details.FirstName = dataReader.GetValue(dataReader.GetOrdinal("FirstName")).ToString();
                details.LastName = dataReader.GetValue(dataReader.GetOrdinal("LastName")).ToString();
                details.Email = dataReader.GetValue(dataReader.GetOrdinal("Email")).ToString();
                details.Username = dataReader.GetValue(dataReader.GetOrdinal("Username")).ToString();
                details.AppUserId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")).ToString());
                details.UserDetailId = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("UserDetailId")).ToString());
            }

            dataReader.Close();
            command.Dispose();
            return details;
        }
    }
}