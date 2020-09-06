using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CodeAtWork.DAL
{
    public class Common
    {
        const string connString = "Data Source=.;Initial Catalog=CodeAtWork;Integrated Security=SSPI;Pooling=False";
        protected SqlConnection conn;

        protected Common()
        {
            conn = new SqlConnection(connString);
            conn.Open();
        }

    }
}