using System.Data;
using System.Data.SqlClient;
using AllPaintsEcomAPI.DTO;
using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AllPaintsEcomAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> VerifyOtpAsync(ParamDto prm)
        {
            //DataSet ds = new DataSet();
            //string query = "sp_get_otp_verify";

            //using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Database")))
            //using (SqlCommand cmd = new SqlCommand(query, con))
            //{
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    for (int i = 1; i <= 15; i++)
            //    {
            //        var prop = prm.GetType().GetProperty($"FilterValue{i}");
            //        var value = prop?.GetValue(prm) ?? DBNull.Value;
            //        cmd.Parameters.AddWithValue($"@FilterValue{i}", value);
            //    }

            //    await con.OpenAsync();
            //    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            //    adapter.Fill(ds);
            //    con.Close();
            //}
            // return JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);


            DataSet ds = new DataSet();
            string query = "sp_get_otp_verify";

            // Example: Decrypt FilterValue1 (assume it's the OTP)
            prm.FilterValue1 = AesEncryption.Decrypt(prm.FilterValue1);

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Database")))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 1; i <= 15; i++)
                {
                    var prop = prm.GetType().GetProperty($"FilterValue{i}");
                    var value = prop?.GetValue(prm) ?? DBNull.Value;
                    cmd.Parameters.AddWithValue($"@FilterValue{i}", value);
                }

                await con.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                con.Close();
            }

            string rawJson = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
            string encryptedJson = AesEncryption.Encrypt(rawJson);
            return encryptedJson;
        }


        public async Task<string> JVFileDtlsData(ParamDto prm)
        {
            DataSet ds = new DataSet();
            string ccode = AesEncryption.Decrypt(prm.FilterValue1);
            string salaryMonth = AesEncryption.Decrypt(prm.FilterValue2);

            string query = @"
            SELECT TOP 1 *
            FROM tbl_jvfile_sap_log
            WHERE CCODE = @Ccode AND SALARY_MNTH = @SalaryMonth
            ORDER BY createddate DESC";

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("Database")))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Ccode", ccode);
                cmd.Parameters.AddWithValue("@SalaryMonth", salaryMonth);

                await con.OpenAsync();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
                con.Close();
            }

            // ✅ Encrypt the result to send back
            string rawJson = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
            string encryptedJson = AesEncryption.Encrypt(rawJson);
            return encryptedJson;
        }


    }
}
