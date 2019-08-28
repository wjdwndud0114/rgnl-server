using rgnl_server.Data;

namespace rgnl_server.Repositories
{
    public class AppRepository
    {
        protected readonly ApplicationDbContext DbContext;

        protected AppRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}