using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AutenticacaoController(IAutenticacaoService servico, IAntiforgery antiforgery)
    : ControllerBase
{
    [HttpGet("csrf")]
    [AllowAnonymous]
    public ActionResult<RetroGamesResponseApiModel> Csrf()
    {
        RenovarTokenCsrf();
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>>> Login(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] LoginRequestApiModel? request)
    {
        var dados = request!;
        var usuario = await servico.AutenticarAsync(dados.Email!, dados.Senha!);
        if (usuario is null)
        {
            var erro = ResponseCodes.CREDENCIAIS_INVALIDAS;
            return Unauthorized(new RetroGamesResponseApiModel
            {
                ResponseCode = erro.Code,
                ErrorMessage = erro.DefaultMessage
            });
        }

        var principal = usuario.ParaPrincipal();
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        HttpContext.User = principal;
        RenovarTokenCsrf();
        return Ok(RespostaUsuario(principal));
    }

    [HttpPost("logout")]
    [Authorize(Policy = Politicas.Administrador)]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
        RenovarTokenCsrf();
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [HttpGet("eu")]
    [Authorize(Policy = Politicas.Administrador)]
    public ActionResult<RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>> Eu()
    {
        return Ok(RespostaUsuario(User));
    }

    private void RenovarTokenCsrf()
    {
        var token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken!;
        Response.Cookies.Append(AutenticacaoConfig.CookieCsrf, token, new CookieOptions
        {
            HttpOnly = false,
            SameSite = SameSiteMode.Strict,
            Secure = Request.IsHttps,
            Path = "/"
        });
    }

    private static RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel> RespostaUsuario(
        ClaimsPrincipal principal)
    {
        return new RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = principal.ParaUsuarioLogado()
        };
    }
}