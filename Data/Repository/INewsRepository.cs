using Newsfeed.Services.Interfaces;
using Newsfeed.Models.DTO;

public interface INewsRepository<T>
{
    Task<IEnumerable<T>> GetAll();

    Task Create(T CreateNewsDTO);

    Task<T> Update(T CreateNewsDTO);

    Task Delete(T CreateNewsDTO);
}
