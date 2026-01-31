using FIAP.CloudGames.Pagamentos.Api.Middlewares;
using Microsoft.OpenApi.Models;
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
            app.UseMiddleware<ForwardedPrefixMiddleware>();

            app.MapControllers();

            // Health
            app.MapHealthChecks("/health");
        }

        private static void UseCustomSwagger(this WebApplication app)
        {
            var pathBase = app.Configuration["Swagger:PathBase"] ?? string.Empty;

            app.UseSwagger(c =>
            {
                if (!string.IsNullOrEmpty(pathBase))
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer>
                        {
                            new OpenApiServer { Url = pathBase }
                        };
                    });
                }
            });

            app.UseSwaggerUI(c =>
            {
                var swaggerUrl = string.IsNullOrEmpty(pathBase) 
                    ? "/swagger/v1/swagger.json" 
                    : $"{pathBase.TrimEnd('/')}/swagger/v1/swagger.json";
            
                c.SwaggerEndpoint(swaggerUrl, "FIAPCloudGames Games API v1");
                
                c.RoutePrefix = "swagger";
                
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