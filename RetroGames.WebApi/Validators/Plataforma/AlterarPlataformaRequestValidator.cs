using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Plataforma;

public sealed class AlterarPlataformaRequestValidator : AbstractValidator<AlterarPlataformaRequestApiModel>
{
    public AlterarPlataformaRequestValidator()
    {
        Include(new InserirPlataformaRequestValidator());
    }
}