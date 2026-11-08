-- Aula 2: SQLs do JogoGeneroRepository.
-- Executar por blocos no DataGrip, na mesma console.

USE retrogames;

-- 1. Listar todos os vínculos.
SELECT co_jogo, co_genero
FROM tb_jogo_genero
ORDER BY co_jogo, co_genero;

-- 2. Listar vínculos de uma lista de jogos.
SET @id_jogo_1 = 1;
SET @id_jogo_2 = 2;

SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo IN (@id_jogo_1, @id_jogo_2)
ORDER BY co_jogo, co_genero;

-- Testes de gravação: desfazer ao terminar.
START TRANSACTION;

SET @id_jogo_teste = 1;

-- 3. Remover os vínculos do jogo informado.
DELETE FROM tb_jogo_genero
WHERE co_jogo = @id_jogo_teste;

-- Esperado: nenhuma linha.
SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo = @id_jogo_teste;

-- 4. Inserir os vínculos do jogo com seus gêneros.
INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES
    (@id_jogo_teste, 1),
    (@id_jogo_teste, 2);

-- Esperado: vínculos (1, 1) e (1, 2).
SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo = @id_jogo_teste
ORDER BY co_genero;

-- Executar separadamente após conferir os resultados.
ROLLBACK;

-- Conferir que os vínculos originais foram preservados.
-- Esperado: vínculos (1, 1) e (1, 2).
SELECT co_jogo, co_genero
FROM tb_jogo_genero
WHERE co_jogo = @id_jogo_teste
ORDER BY co_genero;