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

    public static Plataforma ParaModel(this InserirPlataformaRequestApiModel request)
    {
        return new Plataforma
        {
            Nome = request.Nome!,
            Fabricante = request.Fabricante!,
            AnoLancamento = request.AnoLancamento
        };
    }

    public static Plataforma ParaModel(this AlterarPlataformaRequestApiModel request, int id)
    {
        var plataforma = ((InserirPlataformaRequestApiModel)request).ParaModel();
        plataforma.Id = id;
        return plataforma;
    }
}
