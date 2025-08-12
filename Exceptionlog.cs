using System.Data.SqlClient;

namespace AllPaintsEcomAPI
{
    public class Exceptionlog
    {

        // public Exceptionlog() { }

        public static IConfiguration Configuration;

        public Exceptionlog(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }


        public static void Logexception(string massage, string doctype)
        {
            try
            {
                // doctype =Exceptionlog.Truncate(doctype, 999);

                int maxLength = 1999;
                if (!string.IsNullOrEmpty(doctype) && doctype.Length > maxLength)
                {
                    doctype = doctype.Substring(0, maxLength);
                }


                IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

                string connectionString = configuration.GetConnectionString("Database");


                using (SqlConnection con3 = new SqlConnection("" + connectionString + ""))

                {
                    string query3 = "insert into tbl_allpaints_exceptionlog(massage,createddate,doctype) values (@massage,@createddate,@doctype)";
                    using (SqlCommand cmd3 = new SqlCommand(query3, con3))
                    {
                        cmd3.Parameters.AddWithValue("@massage", massage);
                        cmd3.Parameters.AddWithValue("@createddate", DateTime.Now);
                        cmd3.Parameters.AddWithValue("@doctype", doctype);

                        con3.Open();
                        int iiiii = cmd3.ExecuteNonQuery();
                        if (iiiii > 0)
                        {

                        }
                        con3.Close();
                    }
                }
            }
            catch (Exception ex)
            {
     
            }
            // return msg;
        }
    }
}

