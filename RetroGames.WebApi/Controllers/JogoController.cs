using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class JogoController(IJogoService servico) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>>
        Listar([FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();
        var (jogos, total) = await servico.ListarAsync(
            filtro.Busca, filtro.IdPlataforma, filtro.IdGenero,
            pagina: filtro.Pagina, tamanhoPagina: filtro.TamanhoPagina);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = jogos.ParaPaginaResponse(filtro.Pagina, filtro.TamanhoPagina, total)
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType<RetroGamesResponseApiModel>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>>
        ListarPorId(int id)
    {
        var detalhe = await servico.BuscarPorIdAsync(id);
        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaResponseDetalhe()
        });
    }
}