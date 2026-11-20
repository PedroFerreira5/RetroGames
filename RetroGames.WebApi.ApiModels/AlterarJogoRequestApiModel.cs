namespace RetroGames.WebApi.ApiModels;

public sealed class AlterarJogoRequestApiModel : InserirJogoRequestApiModel
{
    public bool? Ativo { get; init; }
}