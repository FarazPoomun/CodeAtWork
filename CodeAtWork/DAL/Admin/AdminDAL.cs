using CodeAtWork.Models.UI;
using System;
using System.Data.SqlClient;

namespace CodeAtWork.DAL.Admin
{
    public class AdminDAL : CommonDAL
    {
        public AdminDAL() : base() { }


        public Guid SaveNewVideo(CreateVid vid)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();

            string sql = $@"
                DECLARE @INSERTED table ([VideoId] uniqueIdentifier);
                Insert into VideoRepository 
                OUTPUT INSERTED.[VideoId] Into @inserted
                Values (default, '{vid.VideoURL}', '{vid.IsLocal}','{vid.VideoAuthor}','{vid.VideoDescription}'); select * from @inserted;";

            adapter.InsertCommand = new SqlCommand(sql, conn);
            var newId = (Guid)adapter.InsertCommand.ExecuteScalar();

            adapter.Dispose();
            return newId;
        }

    }
}