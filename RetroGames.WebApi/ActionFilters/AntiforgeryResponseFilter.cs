using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public sealed class AntiforgeryResponseFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext contexto)
    {
        if (contexto.Result is IAntiforgeryValidationFailedResult)
        {
            contexto.Result = new BadRequestObjectResult(new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                ErrorMessage = "Token antiforgery ausente ou inválido."
            });
        }
    }

    public void OnResultExecuted(ResultExecutedContext contexto) { }
}