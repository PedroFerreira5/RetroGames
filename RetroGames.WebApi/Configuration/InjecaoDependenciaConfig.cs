using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection servicos, string stringConexao)
    {
        servicos.AddSingleton(new RepositoryConfig { StringConexao = stringConexao });
        servicos.AddScoped<IPlataformaRepository, PlataformaRepository>();
        servicos.AddScoped<IJogoRepository, JogoRepository>();
        servicos.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        servicos.AddScoped<IGeneroRepository, GeneroRepository>();
        servicos.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        servicos.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicos.AddScoped<IRepositorySet, RepositorySet>();
        return servicos;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection servicos)
    {
        servicos.AddScoped<IPlataformaService, PlataformaService>();
        servicos.AddScoped<IJogoService, JogoService>();
        servicos.AddScoped<IGeneroService, GeneroService>();
        servicos.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        servicos.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        servicos.AddScoped<IAutenticacaoService, AutenticacaoService>();
        return servicos;
    }
}
