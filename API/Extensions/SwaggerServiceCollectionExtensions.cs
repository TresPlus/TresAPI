using Microsoft.OpenApi;

namespace API.Extensions
{
  public static class SwaggerServiceCollectionExtensions
  {
    public static IServiceCollection AddTresSwagger(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.Http,
          Scheme = "bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "JWT Bearer token. Örnek kullanım: \n\nBearer {token}"
        });
        c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
          [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
      });
      return services;
    }
  }
}
