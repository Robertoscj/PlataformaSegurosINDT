using Microsoft.EntityFrameworkCore;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Ports;
using ContratacaoService.Infrastructure.ExternalServices;
using ContratacaoService.Infrastructure.Persistence;
using ContratacaoService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Contratacao Service API",
        Version = "v1",
        Description = "API para contratação de propostas de seguro"
    });
});

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ContratacaoDbContext>(options =>
    options.UseNpgsql(connectionString));

// HttpClient para comunicação com PropostaService
var propostaServiceUrl = builder.Configuration["PropostaService:BaseUrl"] 
    ?? "http://localhost:5001";

builder.Services.AddHttpClient<IPropostaServiceClient, PropostaServiceClient>(client =>
{
    client.BaseAddress = new Uri(propostaServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Dependency Injection - Hexagonal Architecture
builder.Services.AddScoped<IContratacaoRepository, ContratacaoRepository>();

// Use Cases
builder.Services.AddScoped<ContratarPropostaUseCase>();
builder.Services.AddScoped<ListarContratacoesUseCase>();
builder.Services.AddScoped<ObterContratacaoUseCase>();

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
    .AddDbContextCheck<ContratacaoDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contratacao Service API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz (http://localhost:5002)
});

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

// Auto-migrate database on startup (apenas para desenvolvimento)
if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ContratacaoDbContext>();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✅ Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Warning: Could not connect to database. Running without database.");
        Console.WriteLine($"   Error: {ex.Message}");
        Console.WriteLine($"   Swagger is available at the configured URL.");
    }
}

app.Run();

// Para testes de integração
public partial class Program { }

