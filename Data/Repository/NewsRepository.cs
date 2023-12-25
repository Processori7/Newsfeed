using IdentityModel;
using Microsoft.EntityFrameworkCore;
using Newsfeed.Models.DTO;
using PostgresDb.Data;

public class NewsRepository : INewsRepository<NewsModel>
{
    private readonly ApiDbContext _apiDbContext;

    public NewsRepository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task<IEnumerable<NewsModel>> GetAll()
    {
        return await _apiDbContext.V3_create_news_table.ToListAsync();
    }
    public async Task Create(NewsModel entity)
    {
        await _apiDbContext.V3_create_news_table.AddAsync(entity);

        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<NewsModel> Update(NewsModel entity)
    {
        _apiDbContext.V3_create_news_table.Update(entity);

        await _apiDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task Delete(NewsModel entity)
    {
        _apiDbContext.V3_create_news_table.Remove(entity);

        await _apiDbContext.SaveChangesAsync();
    }
}
