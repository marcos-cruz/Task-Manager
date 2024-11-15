using System.Reflection;
using System.Text.Json.Serialization;

using Bigai.TaskManager.Application;
using Bigai.TaskManager.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
                .AddJwtBearer();
builder.Services.AddAuthorization();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }