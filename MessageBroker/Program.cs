using Domain.Options;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MessageBroker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<KafkaOption>(options => builder.Configuration.GetSection("Kafka").Bind(options));

            builder.Services.AddDbContext<ChatDbContext>(options =>
                options
                .UseSqlServer(builder.Configuration.GetConnectionString("Chat"))
                .ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning))
            );

            builder.Services.AddHostedService<KafkaConsumer>();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
