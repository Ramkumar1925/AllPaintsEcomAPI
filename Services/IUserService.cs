using AllPaintsEcomAPI.DTO;

namespace AllPaintsEcomAPI.Services
{
    public interface IUserService
    {
     //   UserDto GetUserDtoById(int id);
        Task<string> VerifyOtpAsync(ParamDto prm);
        Task<string> JVFileDtlsData(ParamDto prm);
    };

};
