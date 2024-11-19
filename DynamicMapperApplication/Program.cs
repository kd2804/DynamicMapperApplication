using DynamicMapperApplication;
using DynamicMapperApplication.Services;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Register services
RegisterMappingStrategies(builder.Services);
builder.Services.AddScoped<MapHandler>();

// Register controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Dynamic Mapper API", Version = "v1" });
    options.ExampleFilters(); 
});

// Register example providers from the assembly containing the MappingController
builder.Services.AddSwaggerExamplesFromAssemblyOf<MappingController>();
var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dynamic Mapper API v1");
    });
}

app.MapControllers();

app.Run();

void RegisterMappingStrategies(IServiceCollection services)
{
    services.AddScoped<IMappingStrategy, BookingComMappingStrategy>();
    services.AddScoped<IMappingStrategy, GoogleMappingStrategy>();
}