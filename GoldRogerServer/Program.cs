using GoldRogerServer;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GoldRogerServer.Middleware;

var builder = WebApplication.CreateBuilder(args);
// Configuración del host para definir la URL
builder.WebHost.UseUrls("http://*:5024"); // Agregar esta línea aquí

ServiceConfigurator.ConfigureDBOptions(builder);
ServiceConfigurator.ConfigureRepositories(builder);
ServiceConfigurator.ConfigureBusiness(builder);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

ServiceConfigurator.ConfigureJWTAuth(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ServiceConfigurator.ConfigureSwaggerAuth);
builder.Services.AddHealthChecks();
builder.Services.AddResponseCompression(ServiceConfigurator.ConfigureCompression);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://goldrogerclient-d9ceffhahcffbnaw.canadacentral-01.azurewebsites.net") // Agrega el origen del frontend
              .AllowAnyHeader()
              .AllowAnyMethod() // Esto permite métodos como PUT
              .AllowCredentials(); // Si utilizas autenticación
    });
});
var app = builder.Build();


// Middleware.
app.UseMiddleware<RequestHandlingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseResponseCompression();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.UseCors();
app.MapControllers();
app.Run();