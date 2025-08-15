namespace ProvaPub.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal Value { get; set; }
        public int CustomerId { get; set; }
    }
}
