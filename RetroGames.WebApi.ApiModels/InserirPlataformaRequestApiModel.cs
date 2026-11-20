namespace RetroGames.WebApi.ApiModels;

public class InserirPlataformaRequestApiModel
{
    public string? Nome { get; init; } = string.Empty;
    public string? Fabricante { get; init; } = string.Empty;
    public int AnoLancamento { get; init; }
}