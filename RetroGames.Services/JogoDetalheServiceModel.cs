using RetroGames.Model.Model;

namespace RetroGames.Services;

public record JogoDetalheServiceModel(
    Jogo Jogo,
    string NomePlataforma,
    string DescricaoCondicaoJogo,
    IReadOnlyList<Genero> Generos);