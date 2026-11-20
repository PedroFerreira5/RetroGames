using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public const string CookieCsrf = "XSRF-TOKEN";
    public const string HeaderCsrf = "X-CSRF-TOKEN";

    public static void AddAutenticacao(
        this IServiceCollection servicos, IWebHostEnvironment ambiente)
    {
        var politicaCookie = ambiente.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;

        servicos.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = "retrogames.sessao";
                opcoes.Cookie.HttpOnly = true;
                opcoes.Cookie.SameSite = SameSiteMode.Strict;
                opcoes.Cookie.SecurePolicy = politicaCookie;
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;
                opcoes.Events.OnRedirectToLogin = contexto => ResponderAcessoAsync(
                    contexto.Response, StatusCodes.Status401Unauthorized, ResponseCodes.NAO_AUTENTICADO);
                opcoes.Events.OnRedirectToAccessDenied = contexto => ResponderAcessoAsync(
                    contexto.Response, StatusCodes.Status403Forbidden, ResponseCodes.ACESSO_NEGADO);
            });

        servicos.AddAntiforgery(opcoes =>
        {
            opcoes.HeaderName = HeaderCsrf;
            opcoes.Cookie.Name = "retrogames.antiforgery";
            opcoes.Cookie.HttpOnly = true;
            opcoes.Cookie.SameSite = SameSiteMode.Strict;
            opcoes.Cookie.SecurePolicy = politicaCookie;
        });
    }

    private static Task ResponderAcessoAsync(
        HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage
        });
    }
}