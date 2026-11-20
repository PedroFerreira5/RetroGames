using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using RetroGames.Services;
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
builder.Services.AddAutenticacao(builder.Environment);
builder.Services.AddPoliticasDeAcesso();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services
    .AddControllersWithViews(opcoes =>
    {
        opcoes.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
        // Mantém o envelope antes da conversão de erros do MVC.
        opcoes.Filters.Add<AntiforgeryResponseFilter>(-2100);
        opcoes.Filters.Add<FluentValidationFilter>();
    })
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

await using (var escopo = app.Services.CreateAsyncScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(
        app.Configuration["AdminInicial:Email"],
        app.Configuration["AdminInicial:Senha"]);
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
        opcoes.UseRequestInterceptor("""
            (request) => {
                const url = new URL(request.url, window.location.href);
                if (url.origin !== window.location.origin ||
                    !["POST", "PUT", "DELETE"].includes(request.method?.toUpperCase())) {
                    return request;
                }

                const token = document.cookie.split("; ")
                    .find(cookie => cookie.startsWith("XSRF-TOKEN="));
                if (token) {
                    request.headers["X-CSRF-TOKEN"] = decodeURIComponent(token.slice("XSRF-TOKEN=".length));
                }
                return request;
            }
            """);
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
