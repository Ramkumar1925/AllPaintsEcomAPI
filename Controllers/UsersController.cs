using AllPaintsEcomAPI.DTO;
using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AllPaintsEcomAPI.Services.UserService;

namespace AllPaintsEcomAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }


        //[HttpPost("OtpVerifyed")]

        [Route("OtpVerifyed")]
        [HttpPost]
        public async Task<IActionResult> OtpVerifyed([FromBody] ParamDto prm)
        {
            try
            {
                var resultJson = await _userService.VerifyOtpAsync(prm);
                //return new JsonResult(encryptedJson);

                 return new JsonResult(resultJson);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("GetEmpDtls")]
        [HttpPost]
        public async Task<IActionResult> GetEmpDtls([FromBody] ParamDto prm)
        {
            try
            {
                var encryptedResult = await _userService.JVFileDtlsData(prm);
                return new JsonResult(encryptedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    
    }

}
