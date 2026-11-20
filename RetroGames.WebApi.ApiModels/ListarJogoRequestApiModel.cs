namespace RetroGames.WebApi.ApiModels;

public class ListarJogoRequestApiModel
{
    public string? Busca { get; init; }
    public int? IdPlataforma { get; init; }
    public int? IdGenero { get; init; }
    public bool IncluirInativos { get; init; }

    public int Pagina { get; init; } = 1;

    public int TamanhoPagina { get; init; } = 12;
}