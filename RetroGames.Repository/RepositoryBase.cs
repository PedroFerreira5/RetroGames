using System.Data.Common;
using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _configuracao;

    protected RepositoryBase(RepositoryConfig configuracao)
    {
        _configuracao = configuracao;
    }

    protected async Task<DbConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(
            _configuracao.StringConexao);

        try
        {
            await conexao.OpenAsync();
            return conexao;
        }
        catch
        {
            await conexao.DisposeAsync();
            throw;
        }
    }

    protected DbCommand CriarComando(
        DbConnection conexao,
        string sql)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;

        return comando;
    }

    protected void AdicionarParametro(
        DbCommand comando,
        string nome,
        object? valor)
    {
        var parametro = comando.CreateParameter();
        parametro.ParameterName = nome;
        parametro.Value = valor ?? DBNull.Value;

        comando.Parameters.Add(parametro);
    }

    protected async Task<int> ExecutarGravacaoAsync(
        DbCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException excecao)
            when (excecao.Number == 1062)
        {
            throw new RegistroDuplicadoException(excecao);
        }
    }
}