using System.Reflection;
using System.Text.Json.Serialization;

using Bigai.TaskManager.Application;
using Bigai.TaskManager.Infrastructure;

using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication()
                .AddInfrastructure(builder.Configuration);

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
builder.Services.AddSwaggerGen(options =>
{
    var openApiInfo = builder.Configuration.GetSection(nameof(OpenApiInfo))
                                           .Get<OpenApiInfo>();
    if (openApiInfo is not null)
    {
        options.SwaggerDoc(openApiInfo.Version, openApiInfo);

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        if (xmlFilename is not null)
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        }
    }
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