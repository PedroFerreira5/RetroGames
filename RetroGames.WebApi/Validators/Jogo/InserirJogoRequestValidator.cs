using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Jogo;

public sealed class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(request => request.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(150).WithMessage("O nome deve ter até 150 caracteres.");

        RuleFor(request => request.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter até 1000 caracteres.");

        RuleFor(request => request.AnoLancamento)
            .InclusiveBetween(1970, 2100).WithMessage("Ano de lançamento inválido.");

        RuleFor(request => request.Preco)
            .GreaterThan(0m).WithMessage("O preço deve ser maior que zero.")
            .PrecisionScale(10, 2, true)
            .WithMessage("O preço deve ter até 8 dígitos antes da vírgula e 2 depois.");

        RuleFor(request => request.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque deve ser maior ou igual a zero.");

        RuleFor(request => request.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe a plataforma.");

        RuleFor(request => request.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe a condição do jogo.");

        RuleFor(request => request.IdsGeneros)
            .NotEmpty().WithMessage("Informe pelo menos um gênero.");
    }
}