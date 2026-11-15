using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class PlataformaMapper
{
    public static ListarPlataformaResponseApiModel ParaResponse(this Plataforma plataforma)
    {
        return new ListarPlataformaResponseApiModel(
            plataforma.Id, plataforma.Nome, plataforma.Fabricante, plataforma.AnoLancamento);
    }
}