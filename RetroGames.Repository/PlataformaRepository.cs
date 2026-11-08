using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class PlataformaRepository
    : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = """
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();

        while (await leitor.ReadAsync())
        {
            plataformas.Add(Mapear(leitor));
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT
                co_seq_plataforma,
                no_plataforma,
                no_fabricante,
                nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
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

    public async Task<bool> ExisteComNomeAsync(
        string nome,
        int? idIgnorar = null)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM tb_plataforma
                WHERE no_plataforma = @nome
                  AND (@id_ignorar IS NULL
                       OR co_seq_plataforma <> @id_ignorar)
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@id_ignorar", idIgnorar);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    public async Task<bool> PossuiJogosAsync(int idPlataforma)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM tb_jogo
                WHERE co_plataforma = @id_plataforma
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(
            comando, "@id_plataforma", idPlataforma);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado) == 1;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = """
            INSERT INTO tb_plataforma
                (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES
                (@nome, @fabricante, @ano_lancamento);
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(
            comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(
            comando, "@ano_lancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        await using var comandoId = CriarComando(
            conexao, "SELECT LAST_INSERT_ID();");

        var resultado = await comandoId.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        const string sql = """
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @ano_lancamento
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(
            comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(
            comando, "@ano_lancamento", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);
    }

    public async Task ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await comando.ExecuteNonQueryAsync();
    }

    private static Plataforma Mapear(DbDataReader leitor)
    {
        return new Plataforma
        {
            Id = leitor.GetInt32(
                leitor.GetOrdinal("co_seq_plataforma")),

            Nome = leitor.GetString(
                leitor.GetOrdinal("no_plataforma")),

            Fabricante = leitor.GetString(
                leitor.GetOrdinal("no_fabricante")),

            AnoLancamento = leitor.GetInt32(
                leitor.GetOrdinal("nu_ano_lancamento"))
        };
    }
}