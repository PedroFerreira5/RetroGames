using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepository _repositorio;

    public GeneroService(IRepositorySet repositorio)
    {
        _repositorio = repositorio.Genero;
    }

    public Task<List<Genero>> ListarAsync()
    {
        return _repositorio.ListarAsync();
    }
}