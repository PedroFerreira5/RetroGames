# RetroGames

Projeto acadêmico de uma loja web de jogos retrô, desenvolvido em etapas para praticar desenvolvimento full stack com C#, React e MySQL.

O módulo atual contempla **Catálogo e Administração**: consulta de jogos, manutenção do catálogo e acesso administrativo.

> **Aulas 1 a 7 concluídas.** A API oferece leitura e gravação com autenticação de administrador. O front-end já tem catálogo público com busca, filtros, paginação e detalhe do jogo. As telas administrativas serão desenvolvidas nas próximas aulas.

## Funcionalidades disponíveis

- Catálogo público com busca por nome, filtros por plataforma e gênero e paginação.
- Consulta dos detalhes de um jogo.
- Inclusão, consulta, alteração e exclusão de plataformas pela API.
- Manutenção de jogos pela API, com associação a gêneros e desativação.
- Consulta de gêneros e condições de conservação.
- Sessão de administrador por cookie e proteção antiforgery nas gravações.

## Tecnologias

| Tecnologia | Uso no projeto | Situação |
| --- | --- | --- |
| .NET 10 e ASP.NET Core com controllers | Back-end e API REST | Leitura, gravação e autenticação disponíveis |
| ADO.NET e MySqlConnector 2.6.2 | Acesso ao banco com SQL parametrizado | Repositories implementados |
| MySQL 8 | Persistência relacional | Estrutura e dados de exemplo criados |
| React 19 e Vite 8 | Front-end em JavaScript | Catálogo público e detalhes de jogos |
| xUnit v3 | Testes automatizados | Projeto com o teste inicial do template |
| FluentValidation | Validação das entradas da API | Em uso |
| Bootstrap, Axios e React Router | Interface, comunicação HTTP e navegação | Em uso |

## Arquitetura

O back-end segue a organização em camadas:

```text
React → Controllers → Services → Repositories → MySQL
```

Os Controllers recebem as requisições HTTP. Os Services concentram as regras de negócio, as transações e a composição de dados com LINQ. Os Repositories executam SQL parametrizado e mapeiam os resultados para os modelos C#.

Por uma escolha didática do curso, cada consulta do Repository lê uma única tabela. A composição entre tabelas é feita nos Services. Os exercícios SQL da pasta `database` também incluem consultas com JOIN para estudo e verificação dos dados.

## Estrutura do repositório

```text
RetroGames/
├── database/                       # Estrutura, dados e exercícios SQL
├── docs/                           # Roteiros de validação da API
├── RetroGames.Model/               # Entidades e constantes
├── RetroGames.Repository/          # Acesso a dados e conjunto de repositórios
├── RetroGames.Services/            # Projeto destinado às regras de negócio
├── RetroGames.WebApi.ApiModels/    # Projeto destinado aos contratos da API
├── RetroGames.WebApi/              # Aplicação ASP.NET Core
├── RetroGames.WebApi.UnitTests/    # Projeto de testes
├── retrogames.web/                 # Front-end React
├── .env.example                    # Modelo de configuração local
├── AGENTS.md                       # Diretrizes para assistência de IA
└── RetroGames.sln                  # Solução .NET
```

## Preparar o ambiente

### Pré-requisitos

- .NET SDK 10.
- Node.js 22.12 ou superior da série 22 e npm.
- MySQL Server 8.
- DataGrip ou MySQL Workbench para executar os scripts SQL.
- Git.

### 1. Clonar o projeto

```powershell
git clone https://github.com/PedroFerreira5/RetroGames.git
cd RetroGames
```

### 2. Criar e popular o banco

Na console SQL do DataGrip ou Workbench, conectado ao MySQL local, execute:

```sql
CREATE DATABASE retrogames CHARACTER SET utf8mb4;
USE retrogames;
```

Com o banco `retrogames` selecionado, execute os arquivos nesta ordem:

1. `database/001_criar_tabelas.sql`
2. `database/002_inserir_dados.sql`

Esses scripts são destinados a um banco novo e vazio. A carga de exemplo contém **7 plataformas, 8 gêneros, 20 jogos e 28 vínculos jogo-gênero**, além das condições de conservação e dos perfis. Nenhum usuário é inserido nessa etapa.

O modelo atual possui sete tabelas: `tb_condicao_jogo`, `tb_perfil`, `tb_plataforma`, `tb_genero`, `tb_jogo`, `tb_jogo_genero` e `tb_usuario`.

Os arquivos `003` a `008` contêm exercícios de consultas e testes dos SQLs utilizados no desenvolvimento. Execute-os por blocos e leia os comentários: alguns testes esperam erros de integridade ou utilizam transações que devem ser encerradas com `ROLLBACK`.

### 3. Configurar o MySQL e o administrador

Se ainda não tiver um `.env`, copie o modelo na raiz do projeto:

```powershell
Copy-Item .env.example .env
```

Edite o `.env` com os dados da conexão MySQL. Para habilitar o login administrativo, preencha também `AdminInicial__Email` e `AdminInicial__Senha` com suas credenciais locais.

```dotenv
ConnectionStrings__Default="Server=localhost;Port=3306;Database=retrogames;User ID=root;Password=SUA_SENHA;"
```

O arquivo `.env` é ignorado pelo Git. O `.env.example` deve conter somente valores de exemplo, sem credenciais reais.

A API lê o `.env` na inicialização e cria o administrador se as credenciais estiverem configuradas e o banco ainda não tiver um administrador. O MySQL armazena somente o hash da senha. Alterações no `.env` após a criação da conta preservam as credenciais já registradas no banco.

### 4. Executar a API

Na raiz do projeto, com o MySQL em execução, inicie a API.

```powershell
dotnet run --project RetroGames.WebApi
```

Abra o [Swagger local](http://localhost:5080/swagger) para testar as rotas. Para entrar como administrador, execute `GET /api/Autenticacao/csrf` e depois `POST /api/Autenticacao/login`. O Swagger envia o cabeçalho `X-CSRF-TOKEN` a partir do cookie, e o token é renovado no login e no logout.

### 5. Executar o front-end

```powershell
cd retrogames.web
npm.cmd ci
npm.cmd run dev
```

Em outro terminal, mantenha a API rodando na porta `5080` e abra o [catálogo local](http://localhost:5173). O Vite encaminha as chamadas a `/api` para a API pela mesma origem do navegador. Para encerrar cada processo, pressione `Ctrl+C` no terminal correspondente.

Os comandos acima usam `npm.cmd` para o PowerShell no Windows. Em outros terminais, use `npm`.

## Verificações de desenvolvimento

Na raiz, compile a solução e execute o teste xUnit existente.

```powershell
dotnet build RetroGames.sln
dotnet test RetroGames.sln
```

Verificar e gerar o front-end, a partir de `retrogames.web`:

```powershell
npm.cmd run lint
npm.cmd run build
```

As consultas SQL foram conferidas no DataGrip. A API foi validada com MySQL, e o catálogo e o detalhe foram testados no navegador. Os cenários da API estão documentados em [validação do contrato HTTP](docs/aula-5-validacao-api.md) e [validação do acesso](docs/aula-6-validacao-acesso.md).

## Progresso

- [x] **Aula 1:** ambiente, solução, banco, dados de exemplo, consultas, integridade e modelos C#.
- [x] **Aula 2:** base do Repository, configuração local, Repositories de plataformas, jogos, vínculos, gêneros e condições, e `RepositorySet`.
- [x] **Aula 3:** Services e regras de negócio.
- [x] **Aula 4:** API de leitura e Swagger.
- [x] **Aula 5:** validação e gravação de plataformas e jogos.
- [x] **Aula 6:** autenticação, autorização e proteção antiforgery.
- [x] **Aula 7:** catálogo público e página de detalhes.
- [ ] **Próximas aulas:** telas administrativas e demais funcionalidades do curso.

O histórico do Git registra o avanço por etapa. O quadro de tarefas acompanha as funcionalidades do escopo.

## Autor

Desenvolvido por [Pedro Ferreira](https://github.com/PedroFerreira5), como projeto acadêmico de desenvolvimento web.

As diretrizes para uso de assistência de IA no projeto estão em [AGENTS.md](AGENTS.md).
