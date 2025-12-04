using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using PetStore.Models;          

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// REGISTER DBCONTEXT 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed database
try
{
    DbSeeder.SeedDatabase(app);
    System.Diagnostics.Debug.WriteLine("Database seeding completed");
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Error during database seeding: {ex.Message}");
}

app.Run();