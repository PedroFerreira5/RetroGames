# Aula 5 — Validação do contrato HTTP

A conferência final foi executada contra a API em desenvolvimento, com o Swagger disponível e um banco MySQL temporário preparado com os scripts de criação e carga inicial. Os testes verificaram o status HTTP e o envelope JSON de cada resposta. A simulação de indisponibilidade usou uma instância da API configurada para uma porta local sem servidor MySQL.

| Requisição | HTTP | `responseCode` | Resultado observado |
| --- | --- | --- | --- |
| `GET /api/Plataforma` | 200 | 0 | Lista em `data`. |
| `POST /api/Plataforma` com dados válidos | 201 | 0 | Plataforma criada em `data` e cabeçalho `Location` com o novo ID. |
| `POST /api/Plataforma` com nome vazio e ano 1800 | 400 | 3 | Mensagens em `data.nome` e `data.anoLancamento`. |
| `POST /api/Plataforma` com JSON malformado | 400 | 3 | Mensagem de validação genérica. |
| `GET /api/Jogo/9999` | 404 | 10 | Jogo não encontrado. |
| `POST /api/Plataforma` com nome duplicado | 409 | 8 | Conflito de nome. |
| `DELETE /api/Plataforma/1` com jogos vinculados | 409 | 9 | Conflito de vínculo com jogos. |
| `GET /api/Plataforma` com conexão MySQL indisponível | 500 | 1 | Mensagem genérica sem detalhes internos. |

Os códigos `8`, `9` e `10` seguem a seção 5.3 da especificação técnica adotada no projeto.

## Repetir no Swagger

Na raiz do projeto, execute `dotnet run --project RetroGames.WebApi` e abra [Swagger local](http://localhost:5080/swagger). Use um banco de teste com os dados de exemplo para os cenários de gravação. A plataforma de ID `1` precisa ter jogos vinculados para reproduzir o conflito.

Para o POST válido, use um nome novo, como `Plataforma de teste`, um fabricante e `anoLancamento: 2000`. Repita o mesmo corpo para verificar o erro de duplicidade. Depois de conferir as respostas, exclua a plataforma que você criou usando o ID retornado no POST.