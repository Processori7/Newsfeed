using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using PostgresDb.Data;
using Microsoft.Extensions.Logging;
using System.IO;
using Newsfeed.Services.Interfaces;
using Newsfeed.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adding Serilog logger to the dependency injection container
        var logger = new LoggerConfiguration()
            .WriteTo.File("log.txt")
            .CreateLogger();

        builder.Services.AddSingleton<Serilog.ILogger>(logger);

        // Add services to the container
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        // Authorization and registration service
        builder.Services.AddScoped<IUserServiceAuthorize, UserServiceAuthorize>();

        // User service
        builder.Services.AddScoped<IUserService, UserService>();

        // File service
        builder.Services.AddScoped<IFileService, FileService>();

        // News service
        builder.Services.AddScoped<INewsService, NewsService>();

        builder.Services.AddHttpContextAccessor();

        // Add repositories
        builder.Services.AddScoped<IBaseRepository<RegisterUser>, Repository>();

        builder.Services.AddScoped<INewsRepository<NewsModel>, NewsRepository>();

        // Load environment variables
        DotNetEnv.Env.Load("./.env");

        // Connect to the database

        string connectionString = $"""

        Host={Environment.GetEnvironmentVariable("HOST")};

        Port={Environment.GetEnvironmentVariable("PORT")};

        Database={Environment.GetEnvironmentVariable("DATABASE")};

        Username={Environment.GetEnvironmentVariable("USER_NAME")};

        Password={Environment.GetEnvironmentVariable("PASSWORD")};

        """;

        builder.Services.AddEntityFrameworkNpgsql()
            .AddDbContext<ApiDbContext>(options =>
                options.UseNpgsql(connectionString));

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/v1/news") || context.Request.Path.StartsWithSegments("/v1/user"))
            {
                if (context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "DELETE")
                {
                    if (!UserServiceAuthorize.IsAuthorized)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }
                }
            }
            await next.Invoke();
        });

        app.Run();
    }
}
