using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController(ICondicaoJogoService servico) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>> Listar()
    {
        var itens = await servico.ListarAsync();
        return Ok(new RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = itens.Select(item => item.ParaResponse()).ToList()
        });
    }
}