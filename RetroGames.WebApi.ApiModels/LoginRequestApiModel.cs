namespace RetroGames.WebApi.ApiModels;

public sealed class LoginRequestApiModel
{
    public string? Email { get; init; } = string.Empty;
    public string? Senha { get; init; } = string.Empty;
}