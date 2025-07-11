using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Producer;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ChatDbContext>(options => 
                options
                .UseSqlServer(configuration.GetConnectionString("Chat"))
                .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning))
            );

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddScoped<IKafkaProducer, KafkaProducer>();
        }
    }
}