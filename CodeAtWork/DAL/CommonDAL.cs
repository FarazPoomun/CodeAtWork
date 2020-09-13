using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CodeAtWork.DAL
{
    public class CommonDAL
    {
        const string connString = "Data Source=.;Initial Catalog=CodeAtWork;Integrated Security=SSPI;Pooling=False";
        protected SqlConnection conn;

        protected CommonDAL()
        {
            conn = new SqlConnection(connString);
            conn.Open();
        }

    }
}