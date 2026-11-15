using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class CondicaoJogoMapper
{
    public static ListarCondicaoJogoResponseApiModel ParaResponse(this CondicaoJogo entidade)
    {
        return new ListarCondicaoJogoResponseApiModel(entidade.Id, entidade.Descricao);
    }
}