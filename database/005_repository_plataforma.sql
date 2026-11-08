USE retrogames;

-- 1. Listar plataformas em ordem alfabética.
SELECT
    co_seq_plataforma,
    no_plataforma,
    no_fabricante,
    nu_ano_lancamento
FROM tb_plataforma
ORDER BY no_plataforma;

-- 2. Buscar plataforma pelo ID.
SET @id = 1;

SELECT
    co_seq_plataforma,
    no_plataforma,
    no_fabricante,
    nu_ano_lancamento
FROM tb_plataforma
WHERE co_seq_plataforma = @id;

-- 3. Verificar nome existente, podendo ignorar um ID.
SET @nome = 'Super Nintendo';
SET @id_ignorar = NULL;

SELECT EXISTS (
    SELECT 1
    FROM tb_plataforma
    WHERE no_plataforma = @nome
      AND (@id_ignorar IS NULL
           OR co_seq_plataforma <> @id_ignorar)
) AS existe;

-- 4. Verificar se uma plataforma possui jogos.
SET @id_plataforma = 1;

SELECT EXISTS (
    SELECT 1
    FROM tb_jogo
    WHERE co_plataforma = @id_plataforma
) AS possui_jogos;

-- Executar os blocos por partes, na mesma console.
START TRANSACTION;

-- 5. Inserir uma plataforma de teste.
INSERT INTO tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Plataforma de teste', 'Fabricante de teste', 1990);

SET @id_teste = LAST_INSERT_ID();

SELECT *
FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

-- 6. Alterar a plataforma de teste.
UPDATE tb_plataforma
SET no_plataforma = 'Plataforma de teste alterada',
    no_fabricante = 'Fabricante alterado',
    nu_ano_lancamento = 1991
WHERE co_seq_plataforma = @id_teste;

SELECT *
FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

-- 7. Excluir somente a plataforma de teste.
DELETE FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

SELECT *
FROM tb_plataforma
WHERE co_seq_plataforma = @id_teste;

ROLLBACK;