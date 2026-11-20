using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequestApiModel>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(256).WithMessage("O e-mail deve ter até 256 caracteres.")
            .EmailAddress().WithMessage("Informe um e-mail válido.");

        RuleFor(request => request.Senha)
            .NotEmpty().WithMessage("Informe a senha.");
    }
}