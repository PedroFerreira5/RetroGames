-- Selecionar retrogames no DataGrip antes de executar.
-- Executar somente após 001_criar_tabelas.sql.
-- Não executar novamente se os dados já tiverem sido inseridos.

START TRANSACTION;

INSERT INTO tb_condicao_jogo
    (co_condicao_jogo, ds_condicao_jogo)
VALUES
    (1, 'Novo'),
    (2, 'Usado'),
    (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil)
VALUES
    (1, 'Administrador'),
    (2, 'Cliente');

INSERT INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    (1, 'Super Nintendo', 'Nintendo', 1990),
    (2, 'NES', 'Nintendo', 1983),
    (3, 'Mega Drive', 'Sega', 1988),
    (4, 'Game Boy', 'Nintendo', 1989),
    (5, 'PlayStation', 'Sony', 1994),
    (6, 'Nintendo 64', 'Nintendo', 1996),
    (7, 'Atari 2600', 'Atari', 1977);

INSERT INTO tb_genero (co_seq_genero, no_genero)
VALUES
    (1, 'Plataforma'),
    (2, 'Aventura'),
    (3, 'RPG'),
    (4, 'Corrida'),
    (5, 'Luta'),
    (6, 'Puzzle'),
    (7, 'Ação'),
    (8, 'Esporte');

-- Preços, estoques e condições são dados fictícios para estudo.
INSERT INTO tb_jogo
    (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento,
     vl_preco, qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    (1, 'Super Mario World', 'Aventura na Ilha dos Dinossauros.',
     1990, 149.90, 5, TRUE, UTC_TIMESTAMP(), 1, 2),
    (2, 'Donkey Kong Country', 'Plataforma com Donkey Kong e Diddy Kong.',
     1994, 179.90, 3, TRUE, UTC_TIMESTAMP(), 1, 2),
    (3, 'The Legend of Zelda: A Link to the Past', 'Aventura em Hyrule.',
     1991, 249.90, 2, TRUE, UTC_TIMESTAMP(), 1, 3),
    (4, 'Super Mario Kart', 'Corridas com personagens da Nintendo.',
     1992, 159.90, 4, TRUE, UTC_TIMESTAMP(), 1, 2),
    (5, 'Chrono Trigger', 'RPG com viagens no tempo.',
     1995, 499.90, 1, TRUE, UTC_TIMESTAMP(), 1, 2),
    (6, 'Super Mario Bros.', 'Aventura clássica de Mario.',
     1985, 99.90, 8, TRUE, UTC_TIMESTAMP(), 2, 2),
    (7, 'Mega Man 2', 'Ação e plataforma com o robô Mega Man.',
     1988, 189.90, 2, TRUE, UTC_TIMESTAMP(), 2, 3),
    (8, 'The Legend of Zelda', 'Exploração e aventura em Hyrule.',
     1986, 229.90, 3, TRUE, UTC_TIMESTAMP(), 2, 2),
    (9, 'Sonic the Hedgehog', 'Plataforma em alta velocidade.',
     1991, 119.90, 6, TRUE, UTC_TIMESTAMP(), 3, 2),
    (10, 'Sonic the Hedgehog 2', 'Aventura de Sonic e Tails.',
     1992, 139.90, 4, TRUE, UTC_TIMESTAMP(), 3, 2),
    (11, 'Streets of Rage 2', 'Combate nas ruas para até dois jogadores.',
     1992, 199.90, 2, TRUE, UTC_TIMESTAMP(), 3, 3),
    (12, 'Tetris', 'Quebra-cabeça de encaixe de blocos.',
     1989, 89.90, 7, TRUE, UTC_TIMESTAMP(), 4, 2),
    (13, 'Super Mario Land', NULL,
     1989, 109.90, 3, TRUE, UTC_TIMESTAMP(), 4, 2),
    (14, 'Final Fantasy VII', 'RPG com Cloud e seus aliados.',
     1997, 299.90, 2, TRUE, UTC_TIMESTAMP(), 5, 2),
    (15, 'Crash Bandicoot', 'Aventura de plataforma em ilhas.',
     1996, 129.90, 4, TRUE, UTC_TIMESTAMP(), 5, 2),
    (16, 'Tekken 3', 'Lutas com diferentes estilos de combate.',
     1998, 149.90, 3, TRUE, UTC_TIMESTAMP(), 5, 3),
    (17, 'Tony Hawk''s Pro Skater', 'Manobras e desafios de skate.',
     1999, 119.90, 2, TRUE, UTC_TIMESTAMP(), 5, 2),
    (18, 'Super Mario 64', 'Exploração de mundos em três dimensões.',
     1996, 219.90, 3, TRUE, UTC_TIMESTAMP(), 6, 2),
    (19, 'Mario Kart 64', 'Corridas e batalhas com personagens da Nintendo.',
     1996, 199.90, 0, TRUE, UTC_TIMESTAMP(), 6, 2),
    (20, 'The Legend of Zelda: Ocarina of Time', 'A jornada de Link por Hyrule.',
     1998, 349.90, 1, FALSE, UTC_TIMESTAMP(), 6, 3);

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES
    (1, 1), (1, 2),
    (2, 1), (2, 2),
    (3, 2), (3, 7),
    (4, 4),
    (5, 3),
    (6, 1),
    (7, 1), (7, 7),
    (8, 2), (8, 7),
    (9, 1),
    (10, 1),
    (11, 7),
    (12, 6),
    (13, 1),
    (14, 3),
    (15, 1), (15, 2),
    (16, 5),
    (17, 8),
    (18, 1), (18, 2),
    (19, 4),
    (20, 2), (20, 7);

-- Usuários serão criados pela API na aula de autenticação.
COMMIT;