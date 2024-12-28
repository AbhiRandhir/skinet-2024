using API.Middleware;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    provideOptions => provideOptions.EnableRetryOnFailure()
    );
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();
//Start - Section 12
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var conString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connection string");
    var configuration = ConfigurationOptions.Parse(conString, true);
    
    return ConnectionMultiplexer.Connect(configuration);    
});

builder.Services.AddSingleton<ICartService, CartService>();
//End - Section 12

//Start - Section 14
builder.Services.AddIdentityApiEndpoints<AppUser>()
        .AddEntityFrameworkStores<StoreContext>();
//End - Section 14


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.MapControllers();
//Start - Section 14
app.MapGroup("api").MapIdentityApi<AppUser>(); //api/login, api/register
//End - Section 14


try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex) 
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
