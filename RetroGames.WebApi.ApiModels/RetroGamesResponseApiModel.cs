namespace RetroGames.WebApi.ApiModels;

public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; init; }
    public string? ErrorMessage { get; init; }
}

public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public required T Data { get; init; }
}