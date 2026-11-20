using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Plataforma;

public sealed class InserirPlataformaRequestValidator : AbstractValidator<InserirPlataformaRequestApiModel>
{
    public InserirPlataformaRequestValidator()
    {
        RuleFor(request => request.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(80).WithMessage("O nome deve ter até 80 caracteres.");

        RuleFor(request => request.Fabricante)
            .NotEmpty().WithMessage("Informe o fabricante.")
            .MaximumLength(80).WithMessage("O fabricante deve ter até 80 caracteres.");

        RuleFor(request => request.AnoLancamento)
            .InclusiveBetween(1970, 2100)
            .WithMessage("Ano de lançamento inválido.");
    }
}