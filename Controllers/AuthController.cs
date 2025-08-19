using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AllPaintsEcomAPI;
using AllPaintsEcomAPI.Helpers;
//using AllPaintsAPI.Helper;
using AllPaintsEcomAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AllPaintsEcomAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static Dictionary<string, (string Username, DateTime Expiry)> refreshTokens = new();

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        string iv = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));


        private string GenerateJwtToken(string username, out DateTime expires)
        {
            var audience = _configuration.GetSection("Jwt:Audience").Value;
            var issuer = _configuration.GetSection("Jwt:Issuer").Value;
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value);
            expires = DateTime.Now.AddHours(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = expires,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }





        //[AllowAnonymous]
        //[HttpPost("refresh")]
        //public async Task<IActionResult> Refresh([FromBody] Tokens request)
        //{
        //    if (string.IsNullOrEmpty(request.RefreshToken))
        //        return BadRequest("Refresh token is required");

        //    var refreshTokenHelper = new RefreshTokenHelper(_configuration);
        //    var user = await refreshTokenHelper.GetUserFromRefreshToken(request.RefreshToken);

        //    if (user == null)
        //        return Unauthorized("Invalid refresh token");

        //    var accessToken = GenerateJwtToken(user.UserName, out var tokenExpiry);


        //    var newRefreshToken = GenerateRefreshToken();

        //    var refreshExpiry = DateTime.Now.AddDays(1);

        //    if (!refreshTokenHelper.SaveRefreshTokenToDatabase(user.UserName, newRefreshToken, refreshExpiry))
        //    {
        //        return StatusCode(500, "Failed to save refresh token");
        //    }

        //    return Ok(new Tokens
        //    {
        //        Token = accessToken,
        //        RefreshToken = newRefreshToken,
        //        TokenExpiration = tokenExpiry,
        //        RefreshTokenExpiration = refreshExpiry
        //    });
        //}

        private string GenerateRefreshToken()


        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Login(dynamic users1)
        {

            string encryptedString = Convert.ToString(users1);
            string urlSafe1 = AesEncryption.Decrypt(encryptedString);
            var users = JsonConvert.DeserializeObject<User1>(urlSafe1);

            APIResponse Objresponse = new APIResponse();

            if (users == null || string.IsNullOrEmpty(users.userName) || string.IsNullOrEmpty(users.password))
                return BadRequest("Username and password must be provided.");

            var connStr = _configuration.GetConnectionString("Database");
            string status = string.Empty;
            string firstname = "", employeecode = "", email = "", mobileno = "", cdeptcode = "", cdeptdesc = "",
                cfincode = "", endDate = "", lastname = "", reportManagerid = "", reportMgrPositioncode = "",
                reportMgrPositiondesc = "", reportmanagerName = "", roll_id = "", roll_name = "", username = "", distcode = "";


            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("sp_validate_AllPaints_login_Web", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FilterValue1", users.userName);
                        cmd.Parameters.AddWithValue("@FilterValue2", users.password);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                status = reader["cstatus"]?.ToString() ?? "";
                                if (status == "valid user")
                                {

                                    cdeptcode = reader["cdeptcode"]?.ToString();
                                    cdeptdesc = reader["cdeptdesc"]?.ToString();
                                    cfincode = reader["cfincode"]?.ToString();
                                    email = reader["cfincode"]?.ToString();
                                    employeecode = reader["employeecode"]?.ToString();
                                    endDate = reader["endDate"]?.ToString();
                                    firstname = reader["firstname"]?.ToString();
                                    lastname = reader["lastname"]?.ToString();
                                    mobileno = reader["mobileno"]?.ToString();
                                    reportManagerid = reader["reportManagerid"]?.ToString();
                                    reportMgrPositioncode = reader["reportMgrPositioncode"]?.ToString();
                                    reportMgrPositiondesc = reader["reportMgrPositiondesc"]?.ToString();
                                    reportmanagerName = reader["reportmanagerName"]?.ToString();
                                    roll_id = reader["roll_id"]?.ToString();
                                    roll_name = reader["roll_name"]?.ToString();
                                    username = reader["username"]?.ToString();
                                    distcode = reader["distcode"]?.ToString();
                                }
                            }
                        }
                    }
                }

                if (status == "invalid password")
                {
                    Objresponse.statusText = "Incorrect Password";
                    Objresponse.status = 400;
                    return BadRequest(Objresponse);
                }

                if (status == "username not exist")
                {
                    Objresponse.statusText = "User does not exist.";
                    Objresponse.status = 404;
                    return NotFound(Objresponse);
                }

                if (status == "valid user")
                {
                    var accessToken = GenerateJwtToken(users.userName, out var tokenExpiry);
                    var refreshToken = GenerateRefreshToken();
                    var refreshExpiry = DateTime.Now.AddDays(1);


                    var refreshTokenHelper = new RefreshTokenHelper(_configuration);
                    if (!refreshTokenHelper.SaveRefreshTokenToDatabase(users.userName, refreshToken, refreshExpiry))
                    {
                        return StatusCode(500, "Failed to save refresh token");
                    }


                    var loginDetails = new
                    {

                        cdeptcode = cdeptcode,
                        cdeptdesc = cdeptdesc,
                        cfincode = cfincode,
                        email = email,
                        employeecode = employeecode,
                        endDate = endDate,
                        firstname = firstname,
                        lastname = lastname,
                        mobileno = mobileno,
                        reportManagerid = reportManagerid,
                        reportMgrPositioncode = reportMgrPositioncode,
                        reportMgrPositiondesc = reportMgrPositiondesc,
                        reportmanagerName = reportmanagerName,
                        roll_id = roll_id,
                        roll_name = roll_name,
                        username = username,
                        distcode = distcode,
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        // TokenExpiration = tokenExpiry,
                        // RefreshTokenExpiration = refreshExpiry,


                    };

                    Objresponse.body = new object[] { loginDetails };
                    Objresponse.statusText = "Logged in Successfully";
                    Objresponse.status = 200;
                    // return Ok(EncryptResponse(Objresponse));

                    //return Ok((new APIResponse
                    //{
                    //    status = 200,
                    //    statusText = "Logged in Successfully",
                    //    Body = new[] { loginDetails }
                    //}));
                    var apiDtls = new APIResponse
                    {
                        status = 200,
                        statusText = "Logged in Successfully",
                        body = new[] { loginDetails }
                    };
                    string json = JsonConvert.SerializeObject(apiDtls);
                    var encryptCartDtls1 = AesEncryption.Encrypt(json);
                    return StatusCode(200, encryptCartDtls1);
                }

                Objresponse.statusText = "Unexpected login status";
                Objresponse.status = 500;
                return StatusCode(500, (Objresponse));
            }
            catch (Exception ex)
            {
                Objresponse.statusText = "An error occurred during login: " + ex.Message;
                Objresponse.status = 500;
                return StatusCode(500, (Objresponse));
            }

        }


        [Route("GetEmployeeAsync")]
        [HttpPost]
        public async Task<ActionResult<EmployeenewDTO>> GetEmployeeAsync(dynamic id)
        {
            string encryptedString = Convert.ToString(id);
            string urlSafe1 = AesEncryption.Decrypt(encryptedString);
            var users = JsonConvert.DeserializeObject<UserId>(urlSafe1);

            string query = "select cempno as employeecode,cempname as firstname,'' as lastname,cphoneno as mobileno,cmailid as email,'' as username,'' as password,Roll_id,Roll_name,creportmgrcode,creportmgrname,Roll_Id_mngr,Roll_Id_mngr_desc,cdeptcode,cdeptdesc,(select cfincode from tbl_mis_finyear_mst where bactive=0) cfincode,(select StartDate  from tbl_mis_finyear_mst where bactive=0) StartDate,(select EndDate  from tbl_mis_finyear_mst where bactive=0) EndDate  from Hrm_cempmas where cast(cempno as int) = cast('" + users.id + "' as int)";
            try
            {
                using (SqlConnection con = new SqlConnection(this._configuration.GetConnectionString("Database")))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", users.id);


                        await con.OpenAsync();

                        using (SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                        {
                            if (await sdr.ReadAsync())
                            {
                                var empObj = new EmployeenewDTO
                                {
                                    employeecode = Convert.ToString(sdr["employeecode"]),
                                    firstname = Convert.ToString(sdr["firstname"]),
                                    lastname = Convert.ToString(sdr["lastname"]),
                                    mobileno = Convert.ToString(sdr["mobileno"]),
                                    email = Convert.ToString(sdr["email"]),
                                    username = Convert.ToString(sdr["username"]),
                                    password = Convert.ToString(sdr["password"]),
                                    Roll_id = Convert.ToString(sdr["Roll_id"]),
                                    Roll_name = Convert.ToString(sdr["Roll_name"]),
                                    ReportManagerid = Convert.ToString(sdr["creportmgrcode"]),
                                    ReportmanagerName = Convert.ToString(sdr["creportmgrname"]),
                                    ReportMgrPositioncode = Convert.ToString(sdr["Roll_Id_mngr"]),
                                    ReportMgrPositiondesc = Convert.ToString(sdr["Roll_Id_mngr_desc"]),
                                    cdeptcode = Convert.ToString(sdr["cdeptcode"]),
                                    cfincode = Convert.ToString(sdr["cfincode"]),
                                    StartDate = Convert.ToString(sdr["StartDate"]),
                                    EndDate = Convert.ToString(sdr["EndDate"]),
                                    cdeptdesc = Convert.ToString(sdr["cdeptdesc"])
                                };
                                string json = JsonConvert.SerializeObject(empObj);
                                var encryptCartDtls1 = AesEncryption.Encrypt(json);
                                return new JsonResult(encryptCartDtls1);
                            }
                        }

                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }



    }
}
