using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public sealed class UsuarioRepository(RepositoryConfig configuracao)
    : RepositoryBase(configuracao), IUsuarioRepository
{
    public Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        return BuscarAsync("ds_email = @email", "@email", email);
    }

    public Task<Usuario?> BuscarPorIdAsync(int id)
    {
        return BuscarAsync("co_seq_usuario = @id", "@id", id);
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM tb_usuario WHERE co_perfil = @id_perfil
            );
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@id_perfil", PerfilIds.Administrador);
        return Convert.ToBoolean(await comando.ExecuteScalarAsync());
    }

    public async Task InserirAsync(Usuario usuario)
    {
        const string sql = """
            INSERT INTO tb_usuario
                (no_usuario, ds_email, ds_senha_hash, st_ativo, dt_cadastro, co_perfil)
            VALUES (@nome, @email, @senha_hash, @ativo, @cadastro, @id_perfil);
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, "@nome", usuario.Nome);
        AdicionarParametro(comando, "@email", usuario.Email);
        AdicionarParametro(comando, "@senha_hash", usuario.SenhaHash);
        AdicionarParametro(comando, "@ativo", usuario.Ativo);
        AdicionarParametro(comando, "@cadastro", usuario.Cadastro);
        AdicionarParametro(comando, "@id_perfil", usuario.IdPerfil);
        await ExecutarGravacaoAsync(comando);
    }

    private async Task<Usuario?> BuscarAsync(string filtro, string parametro, object valor)
    {
        // O filtro vem dos métodos deste repositório. O valor sempre vai em um parâmetro.
        var sql = $"""
            SELECT co_seq_usuario, no_usuario, ds_email, ds_senha_hash,
                   st_ativo, dt_cadastro, co_perfil
            FROM tb_usuario
            WHERE {filtro};
            """;

        await using var conexao = await AbrirConexaoAsync();
        await using var comando = CriarComando(conexao, sql);
        AdicionarParametro(comando, parametro, valor);

        await using var leitor = await comando.ExecuteReaderAsync();
        if (!await leitor.ReadAsync())
        {
            return null;
        }

        return new Usuario
        {
            Id = leitor.GetInt32(leitor.GetOrdinal("co_seq_usuario")),
            Nome = leitor.GetString(leitor.GetOrdinal("no_usuario")),
            Email = leitor.GetString(leitor.GetOrdinal("ds_email")),
            SenhaHash = leitor.GetString(leitor.GetOrdinal("ds_senha_hash")),
            Ativo = leitor.GetBoolean(leitor.GetOrdinal("st_ativo")),
            Cadastro = leitor.GetDateTime(leitor.GetOrdinal("dt_cadastro")),
            IdPerfil = leitor.GetInt32(leitor.GetOrdinal("co_perfil"))
        };
    }
}