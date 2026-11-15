using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IPlataformaRepository _plataformas;

    public PlataformaService(IRepositorySet repositorio)
    {
        _plataformas = repositorio.Plataforma;
    }

    public Task<List<Plataforma>> ListarAsync()
    {
        return _plataformas.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        return await _plataformas.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.PlataformaNaoEncontrada);
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        if (await _plataformas.ExisteComNomeAsync(plataforma.Nome))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            plataforma.Id = await _plataformas.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        return plataforma;
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);
        plataforma.Nome = plataforma.Nome.Trim();

        if (await _plataformas.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }

        try
        {
            await _plataformas.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaNomeDuplicado);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await _plataformas.PossuiJogosAsync(id))
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaPossuiJogos);
        }

        await _plataformas.ExcluirAsync(id);
    }
}