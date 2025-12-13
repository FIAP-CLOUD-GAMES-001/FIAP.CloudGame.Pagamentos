using FIAP.CloudGames.Pagamentos.Api.Middlewares;
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

            // Health
            app.MapHealthChecks("/health");
        }

        private static void UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudGames Payments API v1");
                c.SupportedSubmitMethods(new[]
                {
                    SubmitMethod.Get,
                    SubmitMethod.Post,
                    SubmitMethod.Put,
                    SubmitMethod.Delete,
                    SubmitMethod.Patch
            });
            });
        }

    }
}