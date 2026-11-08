using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    private const string Colunas = """
        co_seq_jogo,
        no_jogo,
        ds_jogo,
        nu_ano_lancamento,
        vl_preco,
        qt_estoque,
        st_ativo,
        dt_cadastro,
        co_plataforma,
        co_condicao_jogo
        """;

    public JogoRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        string? busca = null,
        int? idPlataforma = null,
        IReadOnlyCollection<int>? idsJogos = null,
        bool incluirInativos = false,
        int pagina = 1,
        int tamanhoPagina = 5)
    {
        // null significa ausência de filtro.
        // Uma lista vazia significa que nenhum ID corresponde.
        if (idsJogos is not null && idsJogos.Count == 0)
        {
            return (new List<Jogo>(), 0);
        }

        var ids = idsJogos?.ToArray();

        var nomesParametros = Enumerable.Range(
                0, ids?.Length ?? 0)
            .Select(indice => $"@id_jogo_{indice}")
            .ToArray();

        var filtro = """
            WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE)
              AND (@busca IS NULL
                   OR no_jogo LIKE CONCAT('%', @busca, '%'))
              AND (@id_plataforma IS NULL
                   OR co_plataforma = @id_plataforma)
            """;

        if (ids is not null)
        {
            filtro +=
                $"\nAND co_seq_jogo IN ({string.Join(", ", nomesParametros)})";
        }

        var sqlTotal = $"""
            SELECT COUNT(*)
            FROM tb_jogo
            {filtro};
            """;

        var sqlListagem = $"""
            SELECT {Colunas}
            FROM tb_jogo
            {filtro}
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @deslocamento;
            """;

        var deslocamento = checked((pagina - 1) * tamanhoPagina);

        await using var conexao = await AbrirConexaoAsync();

        int total;

        await using (var comandoTotal = CriarComando(
            conexao, sqlTotal))
        {
            AdicionarFiltros(
                comandoTotal, busca, idPlataforma,
                incluirInativos, ids, nomesParametros);

            var resultado = await comandoTotal.ExecuteScalarAsync();
            total = Convert.ToInt32(resultado);
        }

        await using var comando = CriarComando(
            conexao, sqlListagem);

        AdicionarFiltros(
            comando, busca, idPlataforma,
            incluirInativos, ids, nomesParametros);

        AdicionarParametro(comando, "@limite", tamanhoPagina);
        AdicionarParametro(
            comando, "@deslocamento", deslocamento);

        await using var leitor = await comando.ExecuteReaderAsync();

        var jogos = new List<Jogo>();

        while (await leitor.ReadAsync())
        {
            jogos.Add(Mapear(leitor));
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        var sql = $"""
            SELECT {Colunas}
            FROM tb_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return Mapear(leitor);
        }

        return null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento,
                 vl_preco, qt_estoque, st_ativo, dt_cadastro,
                 co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @ano_lancamento,
                 @preco, @estoque, @ativo, @cadastro,
                 @id_plataforma, @id_condicao_jogo);
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarDados(comando, jogo);
        AdicionarParametro(comando, "@cadastro", jogo.Cadastro);

        await ExecutarGravacaoAsync(comando);

        await using var comandoId = CriarComando(
            conexao, "SELECT LAST_INSERT_ID();");

        var resultado = await comandoId.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @ano_lancamento,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                co_plataforma = @id_plataforma,
                co_condicao_jogo = @id_condicao_jogo
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarDados(comando, jogo);
        AdicionarParametro(comando, "@id", jogo.Id);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task DesativarAsync(int id)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = FALSE
            WHERE co_seq_jogo = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await ExecutarGravacaoAsync(comando);
    }

    private void AdicionarFiltros(
        DbCommand comando,
        string? busca,
        int? idPlataforma,
        bool incluirInativos,
        int[]? ids,
        string[] nomesParametros)
    {
        AdicionarParametro(comando, "@busca", busca);
        AdicionarParametro(
            comando, "@id_plataforma", idPlataforma);
        AdicionarParametro(
            comando, "@incluir_inativos", incluirInativos);

        if (ids is null)
        {
            return;
        }

        for (var indice = 0; indice < ids.Length; indice++)
        {
            AdicionarParametro(
                comando, nomesParametros[indice], ids[indice]);
        }
    }

    private void AdicionarDados(DbCommand comando, Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(
            comando, "@ano_lancamento", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(
            comando, "@id_plataforma", jogo.IdPlataforma);
        AdicionarParametro(
            comando, "@id_condicao_jogo", jogo.IdCondicaoJogo);
    }

    private static Jogo Mapear(DbDataReader leitor)
    {
        var indiceDescricao = leitor.GetOrdinal("ds_jogo");

        return new Jogo
        {
            Id = leitor.GetInt32(
                leitor.GetOrdinal("co_seq_jogo")),

            Nome = leitor.GetString(
                leitor.GetOrdinal("no_jogo")),

            Descricao = leitor.IsDBNull(indiceDescricao)
                ? null
                : leitor.GetString(indiceDescricao),

            AnoLancamento = leitor.GetInt32(
                leitor.GetOrdinal("nu_ano_lancamento")),

            Preco = leitor.GetDecimal(
                leitor.GetOrdinal("vl_preco")),

            Estoque = leitor.GetInt32(
                leitor.GetOrdinal("qt_estoque")),

            Ativo = leitor.GetBoolean(
                leitor.GetOrdinal("st_ativo")),

            Cadastro = leitor.GetDateTime(
                leitor.GetOrdinal("dt_cadastro")),

            IdPlataforma = leitor.GetInt32(
                leitor.GetOrdinal("co_plataforma")),

            IdCondicaoJogo = leitor.GetInt32(
                leitor.GetOrdinal("co_condicao_jogo"))
        };
    }
}