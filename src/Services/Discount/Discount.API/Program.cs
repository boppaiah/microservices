using Discount.API;
using Discount.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.RegisterServices(builder.Services);

var app = builder.Build();
app.ApplyMigration<Program>();
startup.SetupMiddleware(app, builder.Environment);

