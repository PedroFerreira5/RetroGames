namespace RetroGames.WebApi.ApiModels;

public class InserirJogoRequestApiModel
{
    public string? Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public int AnoLancamento { get; init; }
    public decimal Preco { get; init; }
    public int Estoque { get; init; }
    public int IdPlataforma { get; init; }
    public int IdCondicaoJogo { get; init; }
    public List<int>? IdsGeneros { get; init; } = [];
}