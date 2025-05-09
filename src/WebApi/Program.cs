using Microsoft.EntityFrameworkCore;
using Model.Ports.Driven;
using Model.Ports.Driving;
using WebApi.Endpoints;
using WebApi;
using Persistence;
using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHostedService<BackgroundWorkerService>();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SabadosTechHexagonal")));

builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddTransient<IRegistrar, Registrar>();
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
builder.Services.AddScoped<ITenantProvider, ConstantTenantProvider>();
builder.Services.AddTransient<INotificator, NullNotificator>();
builder.Services.AddTransient<INotifications, Notifications>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapNotificationsEndpoints();

app.Run();

public partial class Program { }
