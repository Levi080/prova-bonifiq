using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class RandomService
    {
        private static readonly Random _random = new Random();
        private readonly TestDbContext _ctx;

        public RandomService(TestDbContext context)
        {
            _ctx = context;
        }

        public async Task<int> GetRandom()
        {
            int number;
            bool exists;

            do
            {
                number = _random.Next(100);
                exists = await _ctx.Numbers.AnyAsync(n => n.Number == number);

            } while (exists);

            _ctx.Numbers.Add(new RandomNumber() { Number = number });
            await _ctx.SaveChangesAsync();

            return number;
        }
    }
}
