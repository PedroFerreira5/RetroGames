namespace RetroGames.WebApi.ApiModels;

public record ListarPorIdJogoResponseApiModel(
    int Id, string Nome, string? Descricao, int AnoLancamento, decimal Preco,
    int Estoque, bool Ativo, int IdPlataforma, string NomePlataforma,
    int IdCondicaoJogo, string DescricaoCondicaoJogo,
    IReadOnlyList<ListarGeneroResponseApiModel> Generos);