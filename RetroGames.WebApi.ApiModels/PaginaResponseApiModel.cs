namespace RetroGames.WebApi.ApiModels;

public record PaginaResponseApiModel<T>(
    IReadOnlyList<T> Itens, int Pagina, int TamanhoPagina, int TotalItens, int TotalPaginas);