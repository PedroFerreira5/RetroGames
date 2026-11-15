namespace RetroGames.WebApi.ApiModels;

public record ListarPlataformaResponseApiModel(
    int Id,
    string Nome,
    string Fabricante,
    int AnoLancamento);