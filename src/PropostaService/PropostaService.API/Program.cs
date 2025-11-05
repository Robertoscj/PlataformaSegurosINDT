using Microsoft.EntityFrameworkCore;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Ports;
using PropostaService.Infrastructure.Database;
using PropostaService.Infrastructure.Persistence;
using PropostaService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Proposta Service API", 
        Version = "v1",
        Description = "API para gerenciamento de propostas de seguro"
    });
});

// Database - Multi-Provider Support
var dbProvider = builder.Configuration.GetValue<string>("Database:Provider") ?? "PostgreSQL";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var databaseProvider = DatabaseProviderFactory.Create(dbProvider);

builder.Services.AddDbContext<PropostaDbContext>(options =>
{
    databaseProvider.Configure(options, connectionString ?? string.Empty);
    
    // Configurações adicionais do EF Core
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Log do provider sendo usado
builder.Services.AddSingleton(databaseProvider);
Console.WriteLine($"🗄️  Database Provider: {databaseProvider.ProviderName}");

// Dependency Injection - Hexagonal Architecture
builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();

// Use Cases
builder.Services.AddScoped<CriarPropostaUseCase>();
builder.Services.AddScoped<ListarPropostasUseCase>();
builder.Services.AddScoped<ObterPropostaUseCase>();
builder.Services.AddScoped<AlterarStatusPropostaUseCase>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<PropostaDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Auto-migrate database on startup (apenas para desenvolvimento)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PropostaDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();

// Para testes de integração
public partial class Program { }

