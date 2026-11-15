namespace RetroGames.WebApi.ApiModels;

public record ListarJogoResponseApiModel(
    int Id, string Nome, int AnoLancamento, decimal Preco, int Estoque,
    bool Ativo, string NomePlataforma, string DescricaoCondicaoJogo,
    IReadOnlyList<string> Generos);