using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly ICondicaoJogoRepository _repositorio;

    public CondicaoJogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio.CondicaoJogo;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return _repositorio.ListarAsync();
    }
}