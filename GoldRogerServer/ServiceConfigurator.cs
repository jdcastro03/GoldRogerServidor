using GoldRoger.Data;
using GoldRogerServer.Business;
using GoldRogerServer.Business.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace GoldRogerServer
{
    public class ServiceConfigurator
    {
        public static void ConfigureDBOptions(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<GoldRogerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("GoldRogerServerTest")));

        }
        public static void ConfigureRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<UnitOfWork>();
        }
        public static void ConfigureBusiness(WebApplicationBuilder builder)
        {

            builder.Services.AddScoped<UserBusiness>();
            builder.Services.AddScoped<SessionBusiness>();
            builder.Services.AddScoped<PlayerBusiness>();
            builder.Services.AddScoped<RefereeBusiness>();
            builder.Services.AddScoped<OrganizerBusiness>();    
            builder.Services.AddScoped<CoachBusiness>();
            builder.Services.AddScoped<SecurityBusiness>();


        }
        public static void ConfigureJWTAuth(WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        }

        internal static void ConfigureSwaggerAuth(SwaggerGenOptions opt)
        {
            opt.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
        }

        internal static void ConfigureCompression(ResponseCompressionOptions options)
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin",
                    builder => builder.WithOrigins("http://localhost:4200")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials()
                                       .SetIsOriginAllowed(origin => true) // Permitir cualquier origen
              .AllowCredentials()
              );
                                      
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Access-Control-Allow-Origin", "Access-Control-Allow-Methods"));

            app.UseHttpsRedirection();

            // Otro middleware y configuraciones...
        }

    }
}
