# Aula 6 — Validação do acesso à API

Os testes de integração foram executados em uma instância local da API com um banco MySQL temporário, um administrador inicial e uma conta de cliente para verificar o HTTP 403.

| Cenário | HTTP | `responseCode` | Resultado |
| --- | --- | --- | --- |
| POST de plataforma sem sessão | 401 | 4 | A gravação exige login. Os outros cinco endpoints de gravação também retornaram 401. |
| POST de login com senha incorreta | 401 | 6 | Credenciais recusadas. |
| POST de login com e-mail malformado | 400 | 3 | A mensagem de validação aparece em `data.email`. |
| POST de login do administrador | 200 | 0 | Sessão iniciada. A consulta `GET /api/Autenticacao/eu` retorna os dados da conta. |
| POST de plataforma como administrador com token | 201 | 0 | Plataforma criada e depois removida no teste. |
| GET de jogos com `incluirInativos=true`, sem sessão | 200 | 0 | A lista traz os 19 jogos ativos. |
| POST de logout e GET de `/eu` | 200 e 401 | 0 e 4 | A sessão é encerrada e a consulta passa a exigir login. |
| Gravação como cliente autenticado | 403 | 5 | As seis rotas de gravação exigem o perfil Administrador. |
| GET de jogos com `incluirInativos=true`, como administrador | 200 | 0 | A lista inclui o jogo inativo. A consulta direta dele retorna 200 para o administrador e 404 para visitante ou cliente. |
| POST de plataforma como administrador sem token | 400 | 3 | A validação antiforgery rejeita a requisição. |

## Repetir no Swagger

1. Com o administrador inicial configurado no `.env` local, execute `dotnet run --project RetroGames.WebApi` e abra o [Swagger](http://localhost:5080/swagger) em uma janela anônima.
2. Execute `GET /api/Autenticacao/csrf` para obter o token. O Swagger envia o cabeçalho `X-CSRF-TOKEN` nas requisições de gravação.
3. Teste o login e o acesso às rotas da tabela. Login e logout renovam o token. Use um banco de teste para a inclusão de plataforma.