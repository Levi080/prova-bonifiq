using ProvaPub.Payment.Interfaces;

namespace ProvaPub.Payment.Implementations
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        public async Task ProcessPayment(decimal paymentValue)
        {
            Console.WriteLine($"Pagamento de cartão de crédito de {paymentValue} processado.");
            await Task.Delay(100);
        }
    }
}
