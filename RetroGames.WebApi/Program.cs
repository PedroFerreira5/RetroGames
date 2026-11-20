using FluentValidation;
using Microsoft.OpenApi;
using RetroGames.WebApi.ActionFilters;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

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
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services
    .AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
        opcoes.InvalidModelStateResponseFactory = FluentValidationFilter.ResponderModelStateInvalido);
builder.Services.AddOpenApi(opcoes =>
{
    opcoes.AddOperationTransformer((operacao, contexto, _) =>
    {
        var temIdInteiro = contexto.Description.ParameterDescriptions.Any(
            parametro => parametro.Name == "id" && parametro.Type == typeof(int));

        if (temIdInteiro && operacao.Parameters is not null)
        {
            foreach (var parametro in operacao.Parameters.OfType<OpenApiParameter>())
            {
                if (parametro.In == ParameterLocation.Path && parametro.Name == "id")
                {
                    // O Swagger precisa de um único tipo para o campo da rota.
                    parametro.Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Integer,
                        Format = "int32"
                    };
                }
            }
        }

        return Task.CompletedTask;
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes =>
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1"));
}

app.MapControllers();
app.Run();
