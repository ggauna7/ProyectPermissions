using ApplicationCore.Interfaces;
using Confluent.Kafka;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Nest;
using Serilog;
using System.Text.Json.Serialization;
using WebApi.ApplicationCore.Services;
using WebApi.Infrastructure.Data.Repositories;
using WebApi.Infrastructure.ElasticSearch;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console());

var config = new ProducerConfig
{
    BootstrapServers = "localhost:9092"  // Asegúrate de que la dirección del servidor Kafka sea correcta
};

var producer = new ProducerBuilder<string, string>(config).Build();
builder.Services.AddSingleton<IProducer<string, string>>(producer);
builder.Services.AddSingleton<IElasticClient>(provider =>
{
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("permissions");

    return new ElasticClient(settings);
});

// Registrar los repositorios y servicios
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IElasticSearchService, ElasticsearchService>();
builder.Services.AddSingleton<IKafkaService, KafkaService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

// Configurar la cadena de conexión a la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IProducer<string, string>>(new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build());


// Agregar controladores
builder.Services.AddControllers();

// Configurar Swagger si es necesario para la documentación de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
