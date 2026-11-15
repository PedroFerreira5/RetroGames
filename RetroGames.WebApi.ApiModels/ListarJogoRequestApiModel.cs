using System.ComponentModel.DataAnnotations;

namespace RetroGames.WebApi.ApiModels;

public class ListarJogoRequestApiModel
{
    public string? Busca { get; init; }
    public int? IdPlataforma { get; init; }
    public int? IdGenero { get; init; }

    [Range(1, int.MaxValue)]
    public int Pagina { get; init; } = 1;

    [Range(1, int.MaxValue)]
    public int TamanhoPagina { get; init; } = 12;
}