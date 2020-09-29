using CodeAtWork.BL;
using CodeAtWork.Models;
using CodeAtWork.Models.Session;
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

        internal bool ValidateUsername(string userName)
        {
            bool usernameIsInUse = false;
            SqlCommand command;
            SqlDataReader dataReader;

            //TO-DO Accomodate for Email
            string sql = $@"SELECT AppUserId FROM AppUser where username = '{userName}'";

            command = new SqlCommand(sql, conn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (dataReader.GetValue(dataReader.GetOrdinal("AppUserId")) != DBNull.Value)
                {
                    usernameIsInUse = true;
                    break;
                }
            }

            dataReader.Close();
            command.Dispose();
            return usernameIsInUse;
        }

        internal int SaveRegistration(UserDetails user)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"DECLARE @AppUserId int
                            insert into appuser
                            values('{user.Username}', '{user.Password}')

                            SET @AppUserId = SCOPE_IDENTITY()

                            Insert into UserDetail
                            Values (@AppUserId, '{user.FirstName}', '{user.LastName}','{user.Email}')

                            select @AppUserId
                            ";

            adapter.InsertCommand = new SqlCommand(sql, conn);
            var newId = (int)adapter.InsertCommand.ExecuteScalar();
            adapter.Dispose();
            return newId;
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
                if (EncryptionHelper.Decrypt(dataReader.GetValue(dataReader.GetOrdinal("Password")).ToString()) == pwd)
                {
                    validLogin = Convert.ToInt32(dataReader.GetValue(dataReader.GetOrdinal("AppUserId")));
                    break;
                }
            }

            dataReader.Close();
            command.Dispose();
            return validLogin;
        }

    }
}