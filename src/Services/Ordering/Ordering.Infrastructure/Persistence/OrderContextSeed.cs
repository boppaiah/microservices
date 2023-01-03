using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if(!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>
            {
                new Order() {
                    UserName = "bp",
                    FirstName = "Mehmet",
                    LastName = "Ozkaya",
                    EmailAddress = "ezozkme@gmail.com",
                    AddressLine = "Bahcelievler",
                    Country = "Turkey",
                    TotalPrice = 350,
                    CVV ="111",
                    CardName ="Test",
                    CardNumber ="1212121",
                    CreatedBy = "swn",
                    CreatedDate = DateTime.Now,
                    Expiration= DateTime.Now.ToString(),
                    PaymentMethod =(int)PaymentTypes.Card,
                    State ="VIC",
                    ZipCode ="3000",
                    LastModifiedBy ="swn",
                    LastModifiedDate = DateTime.Now,
                }
            };
        }

        public enum PaymentTypes
        {
            Card = 0,
            Check = 1
        }
    }
}
