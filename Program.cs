using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gateway.BlindMatch.Data;
using Gateway.BlindMatch.Models;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection") ?? throw new InvalidOperationException("Connection string 'MongoDbConnection' not found.");
var databaseName = builder.Configuration.GetSection("MongoDbSettings")["DatabaseName"] ?? "BlindMatchDB";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMongoDB(connectionString, databaseName));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityMongoDbProvider<ApplicationUser, MongoRole>(
    identity => {
        identity.SignIn.RequireConfirmedAccount = false;
        identity.Password.RequireDigit = true;
        identity.Password.RequiredLength = 6;
    },
    mongo => { mongo.ConnectionString = connectionString + "/" + databaseName; }
).AddDefaultUI();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndAdminAsync(services);
}

app.Run();
