using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ApplicationFactory : WebApplicationFactory<SmartChargingDbContext>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services =>
        {
            services.AddScoped(sp =>
            {
                return new DbContextOptionsBuilder<SmartChargingDbContext>()
                .UseInMemoryDatabase("Tests", root)
                .UseApplicationServiceProvider(sp)
                .Options;
            });
        });
        return base.CreateHost(builder);
    }
}