using ProvaPub.Models;
using ProvaPub.Payment.Interfaces;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class OrderService
    {
        private readonly TestDbContext _ctx;
        private readonly Dictionary<string, IPaymentStrategy> _strategies;

        public OrderService(TestDbContext ctx, IEnumerable<IPaymentStrategy> strategies)
        {
            _ctx = ctx;
            _strategies = strategies.ToDictionary(
                 s => s.GetType().Name.Replace("PaymentStrategy", "").ToLower(),
                 s => s
                 );
        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int costumerId)
        {
            if (!_strategies.TryGetValue(paymentMethod.ToLower(), out var strategy))
            {
                throw new ArgumentException("Método de pagamento inválido.");
            }

            await strategy.ProcessPayment(paymentValue);


            var order = new Order()
            {
                OrderDate = DateTime.UtcNow,
                Value = paymentValue,
                CustomerId = costumerId
            };
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            return order;
        }
    }
}
