using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartChargingApi.Apis;
using SmartChargingApi.Repository;
using SmartChargingApi.Validators;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder.Services);

var app = builder.Build();

ApplicationInitialization(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
}

app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    //The DI container provides access to the database context and other services
    services.AddDbContext<SmartChargingDbContext>(opt => opt.UseInMemoryDatabase("SmartChargingDataBase"));
    services.AddDatabaseDeveloperPageExceptionFilter();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    services.AddTransient<ISmartChargingRepository, SmartChargingRepository>();
    services.AddTransient<GroupApi>();
    services.AddTransient<ChargeStationApi>();
    services.AddTransient<ConnectorApi>();    

    services.AddScoped<IValidator<GroupModel>, GroupValidator>();
    services.AddScoped<IValidator<ChargeStationModel>, ChargeStationValidator>();
    services.AddScoped<IValidator<ConnectorModel>, ConnectorValidator>();

    builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = builder.Environment.ApplicationName, Version = "v1" }); });
}

void ApplicationInitialization(WebApplication app)
{
    app.Services.GetRequiredService<GroupApi>().Register(app);
    app.Services.GetRequiredService<ChargeStationApi>().Register(app);
    app.Services.GetRequiredService<ConnectorApi>().Register(app);    
}


//For access from unit test
public partial class Program { }

