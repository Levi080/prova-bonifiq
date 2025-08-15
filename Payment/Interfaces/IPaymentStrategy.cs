namespace ProvaPub.Payment.Interfaces
{
    public interface IPaymentStrategy
    {
        Task ProcessPayment(decimal paymentValue);
    }
}
