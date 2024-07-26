using Microsoft.EntityFrameworkCore;
using ShopProjectMVC.Core.Interfaces;
using ShopProjectMVC.Core.Services;
using ShopProjectMVC.Storage;
using ShopProjectMVC.Storage.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Local");

builder.Services.AddDbContext<ShopProjectContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepository, GenericRepository>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IUserService, UserService>();

//builder.Services.AddTransient - creates object every time
//builder.Services.AddScoped    - creates object every request
//builder.Services.AddSingleton - creates object once

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();