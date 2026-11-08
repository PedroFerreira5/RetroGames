using System.Data.Common;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public class JogoGeneroRepository
    : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig configuracao)
        : base(configuracao)
    {
    }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(Mapear(leitor));
        }

        return vinculos;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(
        IReadOnlyCollection<int> idsJogos)
    {
        if (idsJogos.Count == 0)
        {
            return new List<JogoGenero>();
        }

        var ids = idsJogos.ToArray();

        var nomesParametros = Enumerable.Range(0, ids.Length)
            .Select(indice => $"@id_jogo_{indice}")
            .ToArray();

        var sql = $"""
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo IN ({string.Join(", ", nomesParametros)})
            ORDER BY co_jogo, co_genero;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        for (var indice = 0; indice < ids.Length; indice++)
        {
            AdicionarParametro(
                comando, nomesParametros[indice], ids[indice]);
        }

        await using var leitor = await comando.ExecuteReaderAsync();

        var vinculos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            vinculos.Add(Mapear(leitor));
        }

        return vinculos;
    }

    public async Task InserirAsync(
        int idJogo,
        IReadOnlyCollection<int> idsGeneros)
    {
        if (idsGeneros.Count == 0)
        {
            return;
        }

        var ids = idsGeneros.ToArray();

        var nomesParametros = Enumerable.Range(0, ids.Length)
            .Select(indice => $"@id_genero_{indice}")
            .ToArray();

        var linhas = nomesParametros
            .Select(nome => $"(@id_jogo, {nome})");

        var sql = $"""
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES {string.Join(", ", linhas)};
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id_jogo", idJogo);

        for (var indice = 0; indice < ids.Length; indice++)
        {
            AdicionarParametro(
                comando, nomesParametros[indice], ids[indice]);
        }

        await ExecutarGravacaoAsync(comando);
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @id_jogo;
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id_jogo", idJogo);

        await comando.ExecuteNonQueryAsync();
    }

    private static JogoGenero Mapear(DbDataReader leitor)
    {
        return new JogoGenero
        {
            IdJogo = leitor.GetInt32(
                leitor.GetOrdinal("co_jogo")),

            IdGenero = leitor.GetInt32(
                leitor.GetOrdinal("co_genero"))
        };
    }
}