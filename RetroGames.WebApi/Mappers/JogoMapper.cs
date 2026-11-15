using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    public static (string? Busca, int? IdPlataforma, int? IdGenero, int Pagina, int TamanhoPagina)
        ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return (request.Busca, request.IdPlataforma, request.IdGenero,
            request.Pagina, request.TamanhoPagina);
    }

    public static ListarJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;
        return new ListarJogoResponseApiModel(
            jogo.Id, jogo.Nome, jogo.AnoLancamento, jogo.Preco, jogo.Estoque,
            jogo.Ativo, detalhe.NomePlataforma, detalhe.DescricaoCondicaoJogo,
            detalhe.Generos.Select(genero => genero.Nome).ToList());
    }

    public static ListarPorIdJogoResponseApiModel ParaResponseDetalhe(this JogoDetalheServiceModel detalhe)
    {
        var jogo = detalhe.Jogo;
        return new ListarPorIdJogoResponseApiModel(
            jogo.Id, jogo.Nome, jogo.Descricao, jogo.AnoLancamento, jogo.Preco,
            jogo.Estoque, jogo.Ativo, jogo.IdPlataforma, detalhe.NomePlataforma,
            jogo.IdCondicaoJogo, detalhe.DescricaoCondicaoJogo,
            detalhe.Generos.Select(genero => genero.ParaResponse()).ToList());
    }

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaPaginaResponse(
        this IEnumerable<JogoDetalheServiceModel> jogos, int pagina, int tamanhoPagina, int total)
    {
        return new PaginaResponseApiModel<ListarJogoResponseApiModel>(
            jogos.Select(jogo => jogo.ParaResponse()).ToList(), pagina, tamanhoPagina,
            total, (int)Math.Ceiling((decimal)total / tamanhoPagina));
    }
}