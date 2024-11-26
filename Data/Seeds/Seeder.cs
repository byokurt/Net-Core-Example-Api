using System.Threading.Tasks;
using DotnetCoreExampleApi.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotnetCoreExampleApi.Data.Seeds;

public static class Seeder
{
    public static async Task MigrateWithData(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();

        await using DemoDbContext demoDbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();

        await demoDbContext.Database.MigrateAsync();

        User user = new User()
        {
             Name = "Osman",
             Surename = "KURT",
             IsDeleted = false           
        };

        demoDbContext.Users.Add(user);

        await demoDbContext.SaveChangesAsync();
    }

}

