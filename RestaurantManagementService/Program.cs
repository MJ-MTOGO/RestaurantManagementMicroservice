using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagementService.Application.Ports;
using RestaurantManagementService.Application.Services;
using RestaurantManagementService.Infrastructure;
using RestaurantManagementService.Infrastructure.Adapters.Repositories;
using System;

namespace RestaurantManagementService
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
            builder.Services.AddSwaggerGen();

            // Add Database Context with connection string
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            builder.Services.AddDbContext<RestaurantManagementServiceDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

                // Enable detailed SQL logs in development
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging()
                           .EnableDetailedErrors();
                }
            });

            // Register Repository
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();
            builder.Services.AddScoped<IDatabaseRepository, RestaurantRepository>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Add CORS (optional)
            app.UseCors(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
