using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

public sealed class UsuarioAtualService(IHttpContextAccessor contexto) : IUsuarioAtualService
{
    private System.Security.Claims.ClaimsPrincipal? Usuario => contexto.HttpContext?.User;

    public bool EstaAutenticado => Usuario?.Identity?.IsAuthenticated == true;
    public int? Id => Usuario?.ObterIdUsuario();
    public bool EhAdministrador => EstaAutenticado && Usuario?.IsInRole(Perfis.Administrador) == true;
}