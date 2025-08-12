

using System.Data.SqlClient;

namespace AllPaintsEcomAPI.Controllers
{
    internal class RefreshTokenHelper
    {
        //private IConfiguration configuration;

        //public RefreshTokenHelper(IConfiguration configuration)
        //{
        //    this.configuration = configuration;
        //}

        //internal bool SaveRefreshTokenToDatabase(string userName, string refreshToken, DateTime refreshExpiry)
        //{
        //    throw new NotImplementedException();
        //}

        private readonly IConfiguration _configuration;

        public RefreshTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SaveRefreshTokenToDatabase(string userId, string refreshToken, DateTime expiration)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Database")))
                {
                    conn.Open();


                    string checkQuery = "SELECT COUNT(*) FROM RefreshToken WHERE UserId = @UserId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@UserId", userId);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {

                            string updateQuery = @"UPDATE RefreshToken 
                                                SET Token = @Token, 
                                                    Expiration = @Expiration, 
                                                    ModifiedAt = @ModifiedAt 
                                                WHERE UserId = @UserId";

                            using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@Token", refreshToken);
                                updateCmd.Parameters.AddWithValue("@Expiration", expiration);
                                updateCmd.Parameters.AddWithValue("@ModifiedAt", DateTime.Now);
                                updateCmd.Parameters.AddWithValue("@UserId", userId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {

                            string insertQuery = @"INSERT INTO RefreshToken 
                                                (Token, UserId, Expiration, CreatedAt) 
                                                VALUES (@Token, @UserId, @Expiration, @CreatedAt)";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@Token", refreshToken);
                                insertCmd.Parameters.AddWithValue("@UserId", userId);
                                insertCmd.Parameters.AddWithValue("@Expiration", expiration);
                                insertCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<User> GetUserFromRefreshToken(string refreshToken)
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Database")))
            {
                await conn.OpenAsync();
                string query = @"SELECT u.* FROM tbl_mst_employee u 
                               INNER JOIN RefreshToken rt ON u.UserName = rt.UserId 
                               WHERE rt.Token = @Token AND rt.Expiration > @CurrentTime";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Token", refreshToken);
                    cmd.Parameters.AddWithValue("@CurrentTime", DateTime.Now);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),

                            };
                        }
                    }
                }
            }
            return null;
        }

    }
}