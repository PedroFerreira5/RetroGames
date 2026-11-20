using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static ClaimsPrincipal ParaPrincipal(this Usuario usuario)
    {
        var perfil = usuario.IdPerfil switch
        {
            PerfilIds.Administrador => Perfis.Administrador,
            PerfilIds.Cliente => Perfis.Cliente,
            _ => throw new InvalidOperationException("Perfil do usuário inválido.")
        };

        var identidade = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, perfil)
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(identidade);
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        return int.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    }

    public static UsuarioLogadoResponseApiModel ParaUsuarioLogado(this ClaimsPrincipal principal)
    {
        return new UsuarioLogadoResponseApiModel(
            principal.ObterIdUsuario() ?? throw new InvalidOperationException("Sessão sem ID de usuário."),
            principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty);
    }
}