using System.Net;

namespace RetroGames.Services;

public static class BusinessErrors
{
    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PlataformaNaoEncontrada =
        (7, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PlataformaNomeDuplicado =
        (8, "Já existe uma plataforma com este nome.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PlataformaPossuiJogos =
        (9, "Não é possível excluir: existem jogos desta plataforma.", HttpStatusCode.Conflict);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) JogoNaoEncontrado =
        (10, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) PlataformaInexistente =
        (11, "A plataforma informada não existe.", HttpStatusCode.BadRequest);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) CondicaoInexistente =
        (12, "A condição informada não existe.", HttpStatusCode.BadRequest);

    public static readonly (int Code, string DefaultMessage, HttpStatusCode Status) GeneroInexistente =
        (13, "Um ou mais gêneros informados não existem.", HttpStatusCode.BadRequest);
}