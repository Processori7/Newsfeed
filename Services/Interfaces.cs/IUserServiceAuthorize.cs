using Newsfeed.Models.DTO;
using Newsfeed.Models.Response;

namespace Newsfeed.Services.Interfaces;

public interface IUserServiceAuthorize
{
    Task<BaseResponse<RegisterUser>> Register(RegisterUserDTO registerUserDTO);

    Task<BaseResponse<RegisterUser>> Login(AuthUserDTO request);
}
