
using APIs.Exctensions;
using APIs.Helpers;
using APIs.Middelwares;
using Core.Entites.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using StackExchange.Redis;

namespace APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSwagger();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<AppDbContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddServicesExctension(builder.Configuration);

            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });

            #region Cors Ploicy 
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.AllowAnyHeader()                       
                          .AllowAnyMethod()                       
                          .AllowCredentials();                    
                });
            });
            #endregion

            var app = builder.Build();

            #region Updatedatabase
            using var Scope = app.Services.CreateScope();
            var Services = Scope.ServiceProvider;
            var loogerFactory = Services.GetRequiredService<ILoggerFactory>();
            try
            {
                var dbContext = Services.GetRequiredService<AppDbContext>();
                var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
                await dbContext.Database.MigrateAsync();
                
                await AppContxetSeed.SeedAsync(dbContext, roleManager);
                }
            catch (Exception ex)
            {
                var looger = loogerFactory.CreateLogger<Program>();
                looger.LogError(ex, "An Error Occured During Updating The Database");
            }
            #endregion

            app.AddSwagger();
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            #region ErrorHandling
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStatusCodePagesWithRedirects("/errors/{0}");
            #endregion

            app.AddAppExc();
            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
