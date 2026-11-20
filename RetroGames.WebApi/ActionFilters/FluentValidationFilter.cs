using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.ActionFilters;

public sealed class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext contexto, ActionExecutionDelegate proximo)
    {
        foreach (var parametro in contexto.ActionDescriptor.Parameters)
        {
            contexto.ActionArguments.TryGetValue(parametro.Name, out var argumento);

            if (argumento is null)
            {
                if (parametro.BindingInfo?.BindingSource == BindingSource.Body)
                {
                    contexto.Result = ResponderBadRequest(ResponseCodes.MODEL_NULL);
                    return;
                }

                continue;
            }

            var tipoValidador = typeof(IValidator<>).MakeGenericType(parametro.ParameterType);
            if (contexto.HttpContext.RequestServices.GetService(tipoValidador) is not IValidator validador)
            {
                continue;
            }

            var resultado = await validador.ValidateAsync(
                new ValidationContext<object>(argumento),
                contexto.HttpContext.RequestAborted);

            if (resultado.IsValid)
            {
                continue;
            }

            var erros = resultado.Errors
                .GroupBy(erro => ParaCamelCase(erro.PropertyName))
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Select(erro => erro.ErrorMessage).ToArray());

            contexto.Result = new BadRequestObjectResult(
                new RetroGamesResponseApiModel<Dictionary<string, string[]>>
                {
                    ResponseCode = ResponseCodes.MODEL_STATE_INVALIDO.Code,
                    ErrorMessage = ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage,
                    Data = erros
                });
            return;
        }

        await proximo();
    }

    public static IActionResult ResponderModelStateInvalido(ActionContext contexto)
    {
        var esperaCorpo = contexto.ActionDescriptor.Parameters.Any(
            parametro => parametro.BindingInfo?.BindingSource == BindingSource.Body);
        var podeTerCorpo = contexto.HttpContext.Features
            .Get<IHttpRequestBodyDetectionFeature>()?.CanHaveBody;

        return ResponderBadRequest(esperaCorpo && podeTerCorpo == false
            ? ResponseCodes.MODEL_NULL
            : ResponseCodes.MODEL_STATE_INVALIDO);
    }

    private static BadRequestObjectResult ResponderBadRequest(
        (int Code, string DefaultMessage) erro)
    {
        return new BadRequestObjectResult(new RetroGamesResponseApiModel
        {
            ResponseCode = erro.Code,
            ErrorMessage = erro.DefaultMessage
        });
    }

    private static string ParaCamelCase(string propriedade)
    {
        return string.Join('.', propriedade.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
    }
}