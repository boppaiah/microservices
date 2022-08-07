using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            _configuration = configuration;        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {

            using var _npgsqlConnection =  new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var discountCreation = await _npgsqlConnection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)", new {
               ProductName = coupon.ProductName,
               Description = coupon.Description,    
               Amount = coupon.Amount,
           });

            if(discountCreation == 0)
                return false;

            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var _npgsqlConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var deleteCoupon = await _npgsqlConnection.ExecuteAsync("DELETE FROM Coupon WHERE ProductName = @ProductName", new
            {
                ProductName = productName
            });

            if (deleteCoupon == 0)
                return false;

            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            //get the coupon 
            using var _npgsqlConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await _npgsqlConnection.QueryFirstOrDefaultAsync<Coupon>("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });
            if(coupon == null)
            {
                return new Coupon
                {
                    Amount = 0,
                    Description = "No description",
                    ProductName = "No product name"
                };
            }
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var _npgsqlConnection = new NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var updateCoupon = await _npgsqlConnection.ExecuteAsync("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id", new
            {
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
            });

            if (updateCoupon == 0)
                return false;

            return true;
        }
    }
}
