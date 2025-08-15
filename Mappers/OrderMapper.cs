using ProvaPub.DTO;
using ProvaPub.Models;

namespace ProvaPub.Mappers
{
    public class OrderMapper
    {
        public static OrderDto ToOrderDto(Order order)
        {
            if (order == null) return null;

            var brazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            return new OrderDto
            {
                Id = order.Id,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, brazilTimeZone),
                Value = order.Value,
                CustomerId = order.CustomerId
            };
        }
    }
}
