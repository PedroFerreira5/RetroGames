using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] InserirJogoRequestApiModel? request)
    {
        var dados = request!;
        var criado = await servico.InserirAsync(dados.ParaModel(), dados.IdsGeneros!);
        var detalhe = await servico.BuscarPorIdAsync(criado.Id);
        var resposta = new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>
        {
            ResponseCode = ResponseCodes.SUCESSO,
            Data = detalhe.ParaResponseDetalhe()
        };

        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Id }, resposta);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Alterar(
        int id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] AlterarJogoRequestApiModel? request)
    {
        var dados = request!;
        await servico.AlterarAsync(dados.ParaModel(id), dados.IdsGeneros!, dados.Ativo);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await servico.DesativarAsync(id);
        return Ok(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.SUCESSO });
    }
}
