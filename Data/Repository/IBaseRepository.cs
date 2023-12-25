using Newsfeed.Services.Interfaces;
using Newsfeed.Models.DTO;

public interface IBaseRepository<T>
{
    //CRUD Methods
    Task Create(T RegisterUserDTO);

    Task Save();

    Task<IEnumerable<T>> GetAll();

    Task Delete(RegisterUser entity);

}
