using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService : BaseService<Product>
    {
        public ProductService(TestDbContext ctx) : base(ctx) { }
    }
}
