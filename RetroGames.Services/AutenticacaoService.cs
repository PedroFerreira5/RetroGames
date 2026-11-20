using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public sealed class AutenticacaoService(
    IRepositorySet repositorio,
    IPasswordHasher<Usuario> geradorHash) : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario is null || !usuario.Ativo)
        {
            return null;
        }

        var resultado = geradorHash.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(senha))
        {
            return;
        }

        if (await repositorio.Usuario.ExisteAdministradorAsync())
        {
            return;
        }

        var emailConfigurado = email?.Trim();
        if (emailConfigurado is null || emailConfigurado.Length > 256
            || !MailAddress.TryCreate(emailConfigurado, out var endereco)
            || endereco.Address != emailConfigurado
            || string.IsNullOrWhiteSpace(senha) || senha == "SUA_SENHA")
        {
            throw new InvalidOperationException(
                "Configure AdminInicial__Email com um e-mail válido e AdminInicial__Senha no arquivo .env.");
        }

        var administrador = new Usuario
        {
            Nome = Perfis.Administrador,
            Email = emailConfigurado,
            Ativo = true,
            Cadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador
        };
        administrador.SenhaHash = geradorHash.HashPassword(administrador, senha);

        try
        {
            await repositorio.Usuario.InserirAsync(administrador);
        }
        catch (RegistroDuplicadoException)
        {
            // Outra instância pode ter criado o administrador durante esta inicialização.
            if (!await repositorio.Usuario.ExisteAdministradorAsync())
            {
                throw new InvalidOperationException(
                    "O e-mail configurado para o administrador inicial já está em uso.");
            }
        }
    }
}