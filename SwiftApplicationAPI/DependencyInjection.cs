using SwiftApplicationAPI.Services;
using Serilog;
using SwiftApplicationAPI.Data;
using System.Reflection;
using SwiftApplicationAPI.Models.ParseMTModels;
using SwiftApplicationAPI.Services.KafkaServices;
using SwiftApplicationAPI.Services.Currency;
using SwiftApplicationAPI.Services.ParserServices.MT103;
using SwiftApplicationAPI.Services.ParserServices.MT799;
using SwiftApplicationAPI.Services.ParserServices;
using Microsoft.AspNetCore.Identity;
using SwiftApplicationAPI.Models.Users;
using SwiftApplicationAPI.Services.AuthenticationServices;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Confluent.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SwiftApplicationAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
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
            services.AddSingleton<ISwiftParserHelperService, SwiftParserHelperService>();
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            services.AddSingleton<ICurrencyConverterService, CurrencyConverterService>();
            services.AddScoped<ISwiftMessageRepository, SwiftMessageRepository>();
            //Bearer token extractor from the background Services
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextTokenAccessorService, HttpContextTokenAccessorService>();
            //Kafka background host Service
            services.AddHostedService<KafkaConsumerService>();
            //Currency exchanger
            services.AddHttpClient();
            services.AddSingleton<IUserServices, UserServices>();
            services.AddSingleton<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();
            var jwtSettings = configuration.GetSection("Jwt");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });
            services.AddAuthorization();



            return services;
        }
    }
}
