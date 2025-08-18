using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class BaseService<TEntity> where TEntity : class
    {
        protected readonly TestDbContext _ctx;
        protected const int PageSize = 10;

        public BaseService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public PagedList<TEntity> ListItems(int page)
        {
            if (page < 1) page = 1;

            var totalCount = _ctx.Set<TEntity>().Count();
            var items = _ctx.Set<TEntity>()
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            return new PagedList<TEntity>()
            {
                HasNext = (page * PageSize) < totalCount,
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
