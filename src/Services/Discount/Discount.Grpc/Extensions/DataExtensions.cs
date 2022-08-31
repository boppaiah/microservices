using Npgsql;

namespace Discount.Grpc.Extensions
{
    public static class DataExtensions
    {
        public static WebApplication ApplyMigration<TContext>(this WebApplication app, int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = app.Services.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation($"Begin db migration");

                    var _npgsqlConnection = new NpgsqlConnection(config.GetValue<string>("DatabaseSettings:ConnectionString"));
                    _npgsqlConnection.Open();

                    using var command = new NpgsqlCommand() { Connection = _npgsqlConnection };
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated postresql database.");

                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the postresql database");

                    if (retryForAvailability < 50)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        ApplyMigration<TContext>(app, retryForAvailability);
                    }
                }

                return app;
            }
        }
    }
}
