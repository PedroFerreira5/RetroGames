namespace RetroGames.Repository;

public class RegistroDuplicadoException : Exception
{
    public RegistroDuplicadoException(Exception excecaoOriginal)
        : base("Já existe um registro com um valor único informado.",
            excecaoOriginal)
    {
    }
}