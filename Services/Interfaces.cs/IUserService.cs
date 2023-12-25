using Newsfeed.Models.DTO;
using Newsfeed.Models.Response;

namespace Newsfeed.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponse<RegisterUser>> GetAll();

    Task<BaseResponse<RegisterUser>> GetUser(Guid id);

    Task<BaseResponse<RegisterUser>> DeleteUser();

    Task<BaseResponse<RegisterUser>> ChangeData(PutUserDTO user);

    Task<BaseResponse<RegisterUser>> GetUserInfo();
}
