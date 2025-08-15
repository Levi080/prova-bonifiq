using ProvaPub.Payment.Interfaces;

namespace ProvaPub.Payment.Implementations
{
    public class PayPalPaymentStrategy : IPaymentStrategy
    {
        public async Task ProcessPayment(decimal paymentValue)
        {
            Console.WriteLine($"Pagamento Pix de {paymentValue} processado.");
            await Task.Delay(100);
        }
    }
}
