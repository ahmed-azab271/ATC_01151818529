using APIs.Helpers;
using APIs.Helpers.SignalR;
using AutoMapper;
using Core.Entites.Identity;
using Core.IRepositories;
using Core.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Data;
using Services;
using StackExchange.Redis;
using System.Text;

namespace APIs.Exctensions
{
    public static class AddServicesExc
    {
        public static  IServiceCollection AddServicesExctension (this IServiceCollection Services , IConfiguration configuration)
        {
            Services.AddScoped(typeof(IUnitOfWork) , typeof(UnitOfWork));
            Services.AddScoped(typeof(IEventServices) , typeof(EventServices));
            Services.AddScoped(typeof(IEmailSetting), typeof(EmailSettings));
            Services.AddAutoMapper(typeof(MappingProfile));
            Services.AddHostedService<ReminderService>();
            Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            Services.AddSignalR();
            Services.AddSingleton(typeof(ICacheService), typeof(CacheService));
            

            #region For Identity
            Services.AddScoped(typeof(ITokenService), typeof(TokenService));

            Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                };
            });
            #endregion

            return Services;
        }
    }
}
