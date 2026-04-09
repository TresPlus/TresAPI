using API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      builder.Services.AddTresDbServices();

      builder.Services.AddIdentityEntitiesStore(builder.Configuration);

      var AuthBuilder = builder.Services.AddJwtIdentity(builder.Configuration).
          AddExternalProviders(builder.Configuration);

      builder.Services.AddTresSwagger();

      builder.Services.AddControllers();
      builder.Services.AddEndpointsApiExplorer();

      var app = builder.Build();

      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
          c.RoutePrefix = string.Empty;
        });
      }

      //app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseAuthentication();
      app.UseAuthorization();


      app.MapControllers();

      app.Run();
    }
  }
}
