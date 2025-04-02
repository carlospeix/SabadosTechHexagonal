using Microsoft.EntityFrameworkCore;
using Model.Ports.Driven;
using Model.Ports.Driving;
using Model.UseCases;
using Persistence;
using WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<IRegistrar, ApplicationContext>(options =>
    options.UseSqlServer(config.GetConnectionString("SabadosTechHexagonal")));
builder.Services.AddTransient<INotificator, NullNotificator>();
builder.Services.AddTransient<INotifications, Notifications>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapNotificationsEndPoints();

app.Run();
