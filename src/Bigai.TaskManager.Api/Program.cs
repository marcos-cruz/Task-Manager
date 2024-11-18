using System.Reflection;
using System.Text.Json.Serialization;

using Bigai.TaskManager.Api.Middlewares;
using Bigai.TaskManager.Application;
using Bigai.TaskManager.Infrastructure;
using Bigai.TaskManager.Infrastructure.Persistence;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using Serilog;

try
{
    string AllowedHosts = "AllowedHosts";

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAuthentication()
                    .AddJwtBearer();
    builder.Services.AddAuthorization();

    builder.Services.AddScoped<GlobalErrorHandlerMiddleware>();
    builder.Services.AddProblemDetails();

    // Add services to the container.
    builder.Services.AddApplication()
                    .AddInfrastructure(builder.Configuration);

    builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
            );

    builder.Services.AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(swaggerOptions =>
    {
        const string SchemeId = "BearerAuth";
        var openApiInfo = builder.Configuration.GetSection(nameof(OpenApiInfo))
                                               .Get<OpenApiInfo>();
        if (openApiInfo is not null)
        {
            swaggerOptions.SwaggerDoc(openApiInfo.Version, openApiInfo);

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            if (xmlFilename is not null)
            {
                swaggerOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            }
        }

        swaggerOptions.AddSecurityDefinition(SchemeId, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

        swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = SchemeId
                }
            },
            Array.Empty<string>()
        }
        });
    });

    var app = builder.Build();

    app.UseMiddleware<GlobalErrorHandlerMiddleware>();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        await app.Services.InitializeDatabaseAsync();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors(options =>
        {
            var originSettings = builder.Configuration[AllowedHosts];
            if (originSettings is not null)
            {
                options.WithOrigins(originSettings)
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            }
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }