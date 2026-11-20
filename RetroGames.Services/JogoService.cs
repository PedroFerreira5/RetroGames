using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IUsuarioAtualService _usuarioAtual;

    public JogoService(IRepositorySet repositorio, IUsuarioAtualService usuarioAtual)
    {
        _repositorio = repositorio;
        _usuarioAtual = usuarioAtual;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 5)
    {
        int[]? idsJogos = null;

        if (idGenero is int genero)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos
                .Where(vinculo => vinculo.IdGenero == genero)
                .Select(vinculo => vinculo.IdJogo)
                .ToArray();
        }

        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            busca, idPlataforma, idsJogos, incluirInativos && _usuarioAtual.EhAdministrador,
            pagina, tamanhoPagina);

        return (await MontarDetalhesAsync(jogos), total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await BuscarJogoAsync(id);
        if (!jogo.Ativo && !_usuarioAtual.EhAdministrador)
        {
            throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
        }

        var detalhes = await MontarDetalhesAsync([jogo]);
        return detalhes[0];
    }

    public async Task<Jogo> InserirAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        var generos = await ValidarReferenciasAsync(jogo, idsGeneros);
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        jogo.Cadastro = DateTime.UtcNow;

        int id;
        using (var transacao = CriarTransacao())
        {
            id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(id, generos);
            transacao.Complete();
        }

        jogo.Id = id;
        return jogo;
    }

    public async Task AlterarAsync(Jogo jogo, IReadOnlyCollection<int> idsGeneros, bool? ativo = null)
    {
        var jogoAtual = await BuscarJogoAsync(jogo.Id);
        var generos = await ValidarReferenciasAsync(jogo, idsGeneros);
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = ativo ?? jogoAtual.Ativo;

        using (var transacao = CriarTransacao())
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, generos);
            transacao.Complete();
        }
    }

    public async Task DesativarAsync(int id)
    {
        await BuscarJogoAsync(id);
        await _repositorio.Jogo.DesativarAsync(id);
    }

    private async Task<Jogo> BuscarJogoAsync(int id)
    {
        return await _repositorio.Jogo.BuscarPorIdAsync(id)
            ?? throw new BusinessRuleException(BusinessErrors.JogoNaoEncontrado);
    }

    private async Task<int[]> ValidarReferenciasAsync(
        Jogo jogo, IReadOnlyCollection<int> idsGeneros)
    {
        if (await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
        {
            throw new BusinessRuleException(BusinessErrors.PlataformaInexistente);
        }

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(condicao => condicao.Id == jogo.IdCondicaoJogo))
        {
            throw new BusinessRuleException(BusinessErrors.CondicaoInexistente);
        }

        var ids = idsGeneros.Distinct().ToArray();
        var generos = await _repositorio.Genero.ListarAsync();
        var idsValidos = generos.Select(genero => genero.Id).ToHashSet();

        if (ids.Length == 0 || ids.Any(id => !idsValidos.Contains(id)))
        {
            throw new BusinessRuleException(BusinessErrors.GeneroInexistente);
        }

        return ids;
    }

    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        if (jogos.Count == 0)
        {
            return [];
        }

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(
            jogos.Select(jogo => jogo.Id).ToArray());

        var plataformasPorId = plataformas.ToDictionary(plataforma => plataforma.Id);
        var condicoesPorId = condicoes.ToDictionary(condicao => condicao.Id);
        var generosPorId = generos.ToDictionary(genero => genero.Id);
        var vinculosPorJogo = vinculos.ToLookup(vinculo => vinculo.IdJogo);

        return jogos.Select(jogo => new JogoDetalheServiceModel(
            jogo,
            plataformasPorId[jogo.IdPlataforma].Nome,
            condicoesPorId[jogo.IdCondicaoJogo].Descricao,
            vinculosPorJogo[jogo.Id]
                .Select(vinculo => generosPorId[vinculo.IdGenero])
                .ToList())).ToList();
    }

    private static TransactionScope CriarTransacao()
    {
        var opcoes = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout,
        };

        return new TransactionScope(
            TransactionScopeOption.Required,
            opcoes,
            TransactionScopeAsyncFlowOption.Enabled);
    }
}