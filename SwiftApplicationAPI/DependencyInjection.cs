using SwiftApplicationAPI.Services;
using Serilog;
using SwiftApplicationAPI.Services;
using SwiftApplicationAPI.Data;
using System.Reflection;
using SwiftApplicationAPI.Models.ParseMTModels;

namespace SwiftApplicationAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                        .WriteTo
                        .File("logs/log.txt", rollingInterval: RollingInterval.Day)
                        .CreateLogger();


            services.AddLogging(configure =>
            {
                configure.AddConsole().AddSerilog();
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddSingleton<SWIFTMessagesDataContext>();
            services.AddSingleton<ISwiftParserService<MT799Model>, MT799ParserService>();
            services.AddSingleton<ISwiftParserService<MT103Model>, MT103ParserService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            services.AddScoped<ISwiftMessageRepository, SwiftMessageRepository>();



            return services;
        }
    }
}
