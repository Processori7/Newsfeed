using PostgresDb.Data;

public class Repository : IBaseRepository<RegisterUser>
{
    private readonly ApiDbContext _apiDbContext;

    public Repository(ApiDbContext apiDbContext)
    {
        _apiDbContext = apiDbContext;
    }

    public async Task Create(RegisterUser entity)
    {
        await _apiDbContext.V1_create_users_table.AddAsync(entity);

        await _apiDbContext.SaveChangesAsync();
    }

    public async Task Delete(RegisterUser entity)
    {
        _apiDbContext.V1_create_users_table.Remove(entity);

        await _apiDbContext.SaveChangesAsync();
    }
    public async Task Save()
    {
        await _apiDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<RegisterUser>> GetAll()
    {
        return _apiDbContext.V1_create_users_table;
    }
}
