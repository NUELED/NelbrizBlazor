using Microsoft.EntityFrameworkCore;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_Business.Repository;
using Nelbriz_DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Nelbriz_DataAccess;
using NelbrizWeb_Api.Helper;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using NelbrizWeb_Api.Logging;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using NelbrizWeb_Api;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console() // Log to console
    .WriteTo.File("logs/nelbrizlogs.txt", rollingInterval: RollingInterval.Day) // Log to file
    .CreateLogger();

builder.Host.UseSerilog();

//builder.Host.UseSerilog((context, config) =>
//{
//    config.Enrich.FromLogContext();
//    config.ReadFrom.Configuration(context.Configuration);
//});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDependencies(builder.Configuration);


builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddSingleton<ILogging, Logging>(); // This is for the custom implementation!!
builder.Services.AddCors(o => o.AddPolicy("Nelbriz", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));
builder.Services.AddMemoryCache(opt =>
{
    opt.SizeLimit = 1024;
});


//builder.Services.AddHangfire((sp, config) =>
//{
//    var connectstring = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefConnect");
//    config.UseSqlServerStorage(connectstring);
//});
//builder.Services.AddHangfireServer();

var app = builder.Build();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["ApiKey"];

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Nelbriz");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseHangfireDashboard("/jobs/dashboard", new DashboardOptions
//{
//    DashboardTitle = "Nelbriz Hangfire",
//    DarkModeEnabled = false,
//    DisplayStorageConnectionString = false,
//    Authorization = new[]
//    {
//         new HangfireCustomBasicAuthenticationFilter
//         {
//             User = "admin",
//             Pass = "admin123"
//         }
//    }
//});

app.MapControllers();

app.Run();
