using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace BitCoinScraper
{
    class DataAccess
    {
        private static string constring = ConfigurationManager.ConnectionStrings["BCArticles"].ConnectionString;
        public static void InsertRow()
        {
            List<string> roles = new List<string>();
            using (SqlConnection conn = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("GetUserRoles", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", userid);
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        roles.Add(rdr["roletitle"].ToString());
                    }
                }
            }
        }
    }
}
