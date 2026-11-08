-- Aula 2: SQLs do JogoRepository.
-- Executar por blocos no DataGrip, na mesma console.
-- Nos testes de filtros, executar os SET do cenário escolhido
-- e depois repetir somente os SELECT dos blocos A e B.

USE retrogames;

-- =========================================================
-- CONFIGURAÇÃO INICIAL: jogos ativos, sem filtros adicionais.
-- =========================================================

SET @busca = NULL;
SET @id_plataforma = NULL;
SET @incluir_inativos = FALSE;
SET @filtrar_ids = FALSE;
SET @id_jogo_1 = 1;
SET @id_jogo_2 = 2;

-- =========================================================
-- BLOCO A: primeira página.
-- =========================================================

SELECT
    co_seq_jogo,
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
FROM tb_jogo
WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL
       OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@id_plataforma IS NULL
       OR co_plataforma = @id_plataforma)
  AND (@filtrar_ids = FALSE
       OR co_seq_jogo IN (@id_jogo_1, @id_jogo_2))
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 0;

-- =========================================================
-- BLOCO B: total encontrado com os mesmos filtros.
-- =========================================================

SELECT COUNT(*) AS total
FROM tb_jogo
WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL
       OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@id_plataforma IS NULL
       OR co_plataforma = @id_plataforma)
  AND (@filtrar_ids = FALSE
       OR co_seq_jogo IN (@id_jogo_1, @id_jogo_2));

-- =========================================================
-- BLOCO C: segunda página.
-- Com a configuração inicial: cinco jogos e total de 19.
-- Os jogos devem ser diferentes dos da primeira página.
-- =========================================================

SELECT
    co_seq_jogo,
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
FROM tb_jogo
WHERE (@incluir_inativos = TRUE OR st_ativo = TRUE)
  AND (@busca IS NULL
       OR no_jogo LIKE CONCAT('%', @busca, '%'))
  AND (@id_plataforma IS NULL
       OR co_plataforma = @id_plataforma)
  AND (@filtrar_ids = FALSE
       OR co_seq_jogo IN (@id_jogo_1, @id_jogo_2))
ORDER BY no_jogo, co_seq_jogo
LIMIT 5 OFFSET 5;

-- =========================================================
-- CENÁRIO 1: busca por nome.
-- Executar estes SET e depois somente os blocos A e B.
-- Esperado: cinco jogos na primeira página e total de 6.
-- =========================================================

SET @busca = 'Mario';
SET @id_plataforma = NULL;
SET @incluir_inativos = FALSE;
SET @filtrar_ids = FALSE;

-- =========================================================
-- CENÁRIO 2: filtro por plataforma.
-- Executar estes SET e depois somente os blocos A e B.
-- Esperado: cinco jogos do Super Nintendo e total de 5.
-- =========================================================

SET @busca = NULL;
SET @id_plataforma = 1;
SET @incluir_inativos = FALSE;
SET @filtrar_ids = FALSE;

-- =========================================================
-- CENÁRIO 3: filtro por lista de IDs.
-- Executar estes SET e depois somente os blocos A e B.
-- Esperado: jogos 1 e 2; total de 2.
-- =========================================================

SET @busca = NULL;
SET @id_plataforma = NULL;
SET @incluir_inativos = FALSE;
SET @filtrar_ids = TRUE;
SET @id_jogo_1 = 1;
SET @id_jogo_2 = 2;

-- =========================================================
-- CENÁRIO 4: incluir inativos.
-- Executar estes SET e depois somente os blocos A e B.
-- Esperado: cinco jogos na primeira página e total de 20.
-- =========================================================

SET @busca = NULL;
SET @id_plataforma = NULL;
SET @incluir_inativos = TRUE;
SET @filtrar_ids = FALSE;

-- Conferência do jogo inativo.
-- Esperado: Ocarina of Time com st_ativo = 0.
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE st_ativo = FALSE;

-- =========================================================
-- BUSCAR POR ID.
-- Esperado: Super Mario World.
-- =========================================================

SET @id = 1;

SELECT
    co_seq_jogo,
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
FROM tb_jogo
WHERE co_seq_jogo = @id;

-- =========================================================
-- TESTES DE GRAVAÇÃO.
-- Executar por partes, na mesma console.
-- Ao terminar, executar ROLLBACK para desfazer as alterações.
-- =========================================================

START TRANSACTION;

-- INSERIR.
INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento,
     vl_preco, qt_estoque, st_ativo, dt_cadastro,
     co_plataforma, co_condicao_jogo)
VALUES
    ('Teste Repository Jogo', NULL, 1990,
     50.00, 2, TRUE, UTC_TIMESTAMP(),
     1, 2);

SET @id_jogo_teste = LAST_INSERT_ID();

-- Esperado: jogo inserido com preço 50.00.
SELECT *
FROM tb_jogo
WHERE co_seq_jogo = @id_jogo_teste;

-- ALTERAR.
UPDATE tb_jogo
SET no_jogo = 'Teste Repository Jogo alterado',
    ds_jogo = 'Descrição de teste',
    nu_ano_lancamento = 1991,
    vl_preco = 60.00,
    qt_estoque = 3,
    st_ativo = TRUE,
    co_plataforma = 2,
    co_condicao_jogo = 3
WHERE co_seq_jogo = @id_jogo_teste;

-- Esperado: nome alterado, preço 60.00 e estoque 3.
SELECT *
FROM tb_jogo
WHERE co_seq_jogo = @id_jogo_teste;

-- DESATIVAR: não apagar o jogo.
UPDATE tb_jogo
SET st_ativo = FALSE
WHERE co_seq_jogo = @id_jogo_teste;

-- Esperado: registro preservado com st_ativo = 0.
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE co_seq_jogo = @id_jogo_teste;

-- Executar separadamente após conferir as gravações.
ROLLBACK;

-- =========================================================
-- CONFERIR A LIMPEZA.
-- =========================================================

-- Esperado: 20 jogos.
SELECT COUNT(*) AS total
FROM tb_jogo;

-- Esperado: nenhuma linha.
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE co_seq_jogo = @id_jogo_teste;