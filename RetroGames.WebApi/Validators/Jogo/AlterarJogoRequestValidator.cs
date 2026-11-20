using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public sealed class AlterarJogoRequestValidator : AbstractValidator<AlterarJogoRequestApiModel>
{
    public AlterarJogoRequestValidator()
    {
        Include(new InserirJogoRequestValidator());
    }
}