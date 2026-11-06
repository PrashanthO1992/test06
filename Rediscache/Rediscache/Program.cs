using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Rediscache;
using Rediscache.Repositories;
using Rediscache.Service;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var keyVaultUrl = builder.Configuration["KeyVault:Url"];


//if (!string.IsNullOrEmpty(keyVaultUrl))
//{
//    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
//}

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();                      


builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddSingleton<OrderCacheService>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddSingleton<ServiceBusClient>(sp =>

{

    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ServiceBusClient(configuration["AzureAd:ConnectionString"]
        ?? throw new InvalidOperationException("Set AzureAd__ConnectionString in the environment."));

});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetConnectionString("Redis");

    options.InstanceName = "MyApp:";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<OrderSearchService>();
builder.Services.AddScoped<OrderCacheService>();
builder.Services.AddSingleton<Dappercontext>();

var elasticUrl =
    builder.Configuration["Elasticsearch:Url"];

var settings =
    new ElasticsearchClientSettings(
        new Uri(elasticUrl!));

var elasticClient =
    new ElasticsearchClient(settings);

builder.Services.AddSingleton(elasticClient);

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
