-- Teste 1: jogo com plataforma inexistente.
-- Esperado: erro 1452 (chave estrangeira).
-- Resultado observado: preencher após executar.

START TRANSACTION;

INSERT INTO retrogames.tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    ('Teste de plataforma inexistente', 1990, 10.00, 1,
     TRUE, UTC_TIMESTAMP(), -1, 2);

ROLLBACK;


-- Teste 2: plataforma com nome já cadastrado.
-- Esperado: erro 1062 (UNIQUE).
-- Resultado observado: preencher após executar.

START TRANSACTION;

INSERT INTO retrogames.tb_plataforma
    (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES
    ('Super Nintendo', 'Nintendo', 1990);

ROLLBACK;


-- Teste 3: excluir plataforma que possui jogos.
-- Esperado: erro 1451 (chave estrangeira).
-- Resultado observado: preencher após executar.

START TRANSACTION;

DELETE FROM retrogames.tb_plataforma
WHERE co_seq_plataforma = 1;

ROLLBACK;

-- Teste 4: inserir jogo com nome nulo.
-- Esperado: erro 1048 (NOT NULL).
-- Resultado observado: preencher após executar.

START TRANSACTION;

INSERT INTO retrogames.tb_jogo
    (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
     st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES
    (NULL, 1990, 10.00, 1,
     TRUE, UTC_TIMESTAMP(), 1, 2);

ROLLBACK;


-- Resultado observado: inserção aceita com preço zero;
-- registro de teste removido por ROLLBACK.