using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext contexto, Exception excecao, CancellationToken cancelamento)
    {
        RetroGamesResponseApiModel resposta;

        if (excecao is BusinessRuleException regra)
        {
            contexto.Response.StatusCode = (int)regra.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = regra.Erro.Code,
                ErrorMessage = regra.Message
            };
        }
        else
        {
            logger.LogError(excecao, "Erro inesperado ao processar a requisição.");
            contexto.Response.StatusCode = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro interno no servidor."
            };
        }

        await contexto.Response.WriteAsJsonAsync(resposta, cancelamento);
        return true;
    }
}