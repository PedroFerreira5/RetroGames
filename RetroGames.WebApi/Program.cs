using RetroGames.WebApi.Configuration;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var stringConexao = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(stringConexao))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings__Default no arquivo .env na raiz do RetroGames.");
}

builder.Services.AddRepositorios(stringConexao);
builder.Services.AddServicosDeNegocio();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes =>
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1"));
}

app.MapControllers();
app.Run();
