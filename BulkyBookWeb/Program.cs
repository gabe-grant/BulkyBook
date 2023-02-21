
// This file is responsible for running the application

using BulkyBook.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the (dependency injection) container.
builder.Services.AddControllersWithViews();

// Adding the SQL Server NuGet packages allows us to connect the database 
// the 'DefaultConnection' string arg correlates to the key inside ConnectionStrings in appsetting.json
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline (middleware setup)
// the pipeline specifies how the application should respond to a web request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Order is important here (i.e. Authentication before Authorization)
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// MVC is a middleware itself
// this routes our requests to the respective controllers and action methods
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
