using FIAP.CloudGames.Pagamentos.Api.Middlewares;
using FIAP.CloudGames.Pagamentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace FIAP.CloudGames.Pagamentos.Api.Extensions
{
    public static class AppExtension
    {
        public static void UseProjectConfiguration(this WebApplication app)
        {
            app.UseCustomSwagger();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.MapControllers();
            app.GenerateMigrations();
            app.MapHealthChecks("/health");
        }

        private static void UseCustomSwagger(this WebApplication app)
        {
            //if (!app.Environment.IsDevelopment())
            //    return;

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FiapCloudGamesApi API v1");
                c.SupportedSubmitMethods([
                    SubmitMethod.Get,
                SubmitMethod.Post,
                SubmitMethod.Put,
                SubmitMethod.Delete,
                SubmitMethod.Patch
                ]);
            });
        }
        private static void GenerateMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.Migrate();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var email = config[": 'There is already an object : 'There is already an object named 'Users' in the database.'named 'U: 'There is already an object named 'Users' in the database.'sers' in the database.'SeedAdmin:Email"] ?? Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL");
            var password = config["SeedAdmin:Password"] ?? Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD");


        }
    }
}
