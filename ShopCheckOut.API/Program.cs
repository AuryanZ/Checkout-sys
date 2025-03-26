using ShopCheckOut.API.Data.Discounts;
using ShopCheckOut.API.Data.Orders;
using ShopCheckOut.API.Data.Products;
using ShopCheckOut.API.Data.Services.Discounts;
using ShopCheckOut.API.Data.Services.Orders;
using ShopCheckOut.API.Data.Services.Products;
using System.Text.Json.Serialization;

void ConfigureServices(IServiceCollection services)
{
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.AddScoped<IProductsRepo, ProductsRepo>();
    services.AddScoped<IOrderRepo, OrderRepo>();
    services.AddScoped<IDiscountRepo, DiscountRepo>();

    services.AddScoped<IProductServices, ProductServices>();
    services.AddScoped<IDiscountService, DiscountService>();
    services.AddScoped<IOrderService, OrderService>();
}

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
