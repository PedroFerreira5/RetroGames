USE retrogames;

-- Listar gêneros em ordem alfabética.
SELECT co_seq_genero, no_genero
FROM tb_genero
ORDER BY no_genero;

-- Listar condições de jogo.
SELECT co_condicao_jogo, ds_condicao_jogo
FROM tb_condicao_jogo
ORDER BY co_condicao_jogo;