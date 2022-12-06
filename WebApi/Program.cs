using Data;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using WebApi.Constants;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "Standart Authorization header using the Bearer scheme, e.g. \"bearer {token} \"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                }); 

                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration[ConfigurationConstants.TokenSettings_TokenString])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            builder.Services.AddTransient<IUnitOfWork>(
                s => new UnitOfWork(new Data.Context.SpendNoteContext(
                    builder.Configuration.GetConnectionString("MSSQL"))));

            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}