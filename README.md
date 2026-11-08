# RetroGames

Projeto acadêmico de uma loja web de jogos retrô, desenvolvido em etapas para praticar desenvolvimento full stack com C#, React e MySQL.

O módulo atual contempla **Catálogo e Administração**: consulta de jogos, manutenção do catálogo e acesso administrativo.

> **Em desenvolvimento:** as Aulas 1 e 2 estão concluídas. O banco, os modelos e a camada Repository estão implementados. A integração com os Services, os endpoints do catálogo e as telas de negócio será construída nas próximas aulas. Atualmente, o front-end exibe a página inicial do template React + Vite.

## Funcionalidades previstas

- Catálogo público com busca por nome, filtros por plataforma e gênero e paginação.
- Consulta dos detalhes de um jogo.
- Inclusão, consulta, alteração e exclusão de plataformas.
- Manutenção de jogos, associação a gêneros e exclusão lógica.
- Consulta de gêneros e condições de conservação.
- Autenticação e autorização do perfil Administrador.

## Tecnologias

| Tecnologia | Uso no projeto | Situação |
| --- | --- | --- |
| .NET 10 e ASP.NET Core com controllers | Back-end e API REST | Solução criada |
| ADO.NET e MySqlConnector 2.6.2 | Acesso ao banco com SQL parametrizado | Repositories implementados |
| MySQL 8 | Persistência relacional | Estrutura e dados de exemplo criados |
| React 19 e Vite 8 | Front-end em JavaScript | Estrutura inicial criada |
| xUnit v3 | Testes automatizados | Projeto de testes criado |
| FluentValidation e Moq | Validação e simulação de dependências nos testes | Previstos na especificação |
| Bootstrap, Axios e React Router | Interface, comunicação HTTP e navegação | Previstos na especificação |

## Arquitetura

O back-end segue a organização em camadas:

```text
React → Controllers → Services → Repositories → MySQL
```

Os Controllers recebem as requisições HTTP. Os Services concentrarão as regras de negócio, as transações e a composição de dados com LINQ. Os Repositories executam SQL parametrizado e mapeiam os resultados para os modelos C#.

Por uma escolha didática do curso, cada consulta do Repository lê uma única tabela. A composição entre tabelas será feita nos Services. Os exercícios SQL da pasta `database` também incluem consultas com JOIN para estudo e verificação dos dados.

## Estrutura do repositório

```text
RetroGames/
├── database/                       # Estrutura, dados e exercícios SQL
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

### 3. Configurar a conexão local

Na raiz do projeto, copie o modelo:

```powershell
Copy-Item .env.example .env
```

Edite apenas o `.env`, preenchendo a senha local e ajustando os dados da conexão quando necessário:

```dotenv
ConnectionStrings__Default="Server=localhost;Port=3306;Database=retrogames;User ID=root;Password=SUA_SENHA;"
```

O arquivo `.env` é ignorado pelo Git. O `.env.example` deve conter somente valores de exemplo, sem credenciais reais.

**No estágio atual, a API ainda não carrega o `.env` nem injeta a configuração nos Repositories.** Essa integração será feita em uma aula posterior.

### 4. Compilar o back-end

Na raiz do projeto:

```powershell
dotnet build RetroGames.sln
```

### 5. Executar o front-end

```powershell
cd retrogames.web
npm.cmd ci
npm.cmd run dev
```

Abra o endereço informado pelo Vite no terminal, normalmente `http://localhost:5173`. Mantenha o processo em execução enquanto utiliza a página; para encerrá-lo, pressione `Ctrl+C`.

Os comandos acima usam `npm.cmd` para o PowerShell no Windows. Em outros terminais, use `npm`.

## Verificações de desenvolvimento

Compilar a solução .NET, a partir da raiz:

```powershell
dotnet build RetroGames.sln
```

Verificar e gerar o front-end, a partir de `retrogames.web`:

```powershell
npm.cmd run lint
npm.cmd run build
```

As consultas e os testes SQL das Aulas 1 e 2 foram conferidos manualmente no DataGrip, e a solução compilou durante as etapas. A execução integrada dos métodos C# contra o banco e os testes automatizados das regras de negócio ainda fazem parte das próximas etapas.

## Progresso

- [x] **Aula 1:** ambiente, solução, banco, dados de exemplo, consultas, integridade e modelos C#.
- [x] **Aula 2:** base do Repository, configuração local, Repositories de plataformas, jogos, vínculos, gêneros e condições, e `RepositorySet`.
- [ ] **Próximas aulas:** camada Service, integração da API, validação, autenticação, telas e testes das funcionalidades.

O histórico do Git registra o avanço por etapa. O quadro de tarefas acompanha as funcionalidades do escopo.

## Autor

Desenvolvido por [Pedro Ferreira](https://github.com/PedroFerreira5), como projeto acadêmico de desenvolvimento web.

As diretrizes para uso de assistência de IA no projeto estão em [AGENTS.md](AGENTS.md).
