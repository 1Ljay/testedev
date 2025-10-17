# TesteDev.WinForms

Aplicativo Windows Forms (.NET 8) com CRUD simples em PostgreSQL. A aplicação permite cadastrar pessoas com `nome` e `idade`, realizando operações de inserir, atualizar, excluir e listar com filtro.

## Tecnologias

- C# / .NET 8 (Windows Forms)
- PostgreSQL 13+
- Npgsql (driver PostgreSQL para .NET)

## Requisitos

- Windows 10+
- [.NET SDK 8.x](https://dotnet.microsoft.com/)
- PostgreSQL 13+ instalado e em execução
- Usuário do banco com permissão de criação (para rodar `schema.sql`)

## Configuração do Banco de Dados

1. Abra o pgAdmin e crie (se necessário) o banco `TesteDev`.
2. No pgAdmin (Query Tool), execute o arquivo `schema.sql` localizado na raiz do projeto.
   - O script cria as tabelas `public.cadastro` e `public.log_operacoes`, a função/trigger de log, e o usuário `app_user` com as permissões adequadas.
3. O `schema.sql` também insere alguns dados iniciais para teste.

Caso queira alterar o usuário/senha do banco, ajuste a connection string:

- Arquivo: `TesteDev.WinForms/appsettings.json`
- Chave: `ConnectionStrings.Pg`

Exemplo padrão utilizado pela aplicação:

```
Host=localhost;Port=5432;Database=TesteDev;Username=app_user;Password=SenhaForte!123;Pooling=true;
```

Se o `appsettings.json` não for encontrado no diretório de execução, a aplicação usará a mesma connection string acima como fallback.

## Executando o Projeto

1. Abra o repositório no Visual Studio 2022+ ou no VS Code (com extensões C#).
2. Garanta que o banco `TesteDev` esteja com o schema criado (rodar `schema.sql`).
3. Execute o projeto `TesteDev.WinForms` (F5). A janela principal (`MainForm`) abrirá.
4. Utilize os campos:
   - Nome (texto)
   - Idade (numérica, positiva; pode ficar vazia, será enviada como NULL)
   - Botões: `Novo`, `Salvar`, `Excluir`, e campo/botão `Buscar` (busca por idade exata ou nome por aproximação, sem acentos se a extensão unaccent estiver disponível; a aplicação faz fallback caso não esteja).

## Estrutura do Projeto

```
TesteDev.WinForms.sln                  # Solução
TesteDev.WinForms/                     # Projeto WinForms
  Program.cs                           # Inicialização e leitura de appsettings.json
  MainForm.cs                          # Lógica da tela, CRUD e validações
  MainForm.Designer.cs                 # Layout da tela
  appsettings.json                     # Connection string (opcional em runtime)
schema.sql                              # Script completo de criação do schema
```

### Esquema do Banco (resumo)

- `public.cadastro`
  - `id` (IDENTITY, PK)
  - `nome` (TEXT, NOT NULL, `ck_nome_not_blank`)
  - `idade` (INTEGER, NULL; `ck_idade_pos` assegura `idade > 0` quando informado; `uq_idade` impede repetição)
  - Índice: `idx_cadastro_nome` em `lower(nome)` para acelerar buscas por nome
- `public.log_operacoes`
  - Log simples de `INSERT`, `UPDATE`, `DELETE` em `cadastro`, com `instante` (TIMESTAMPTZ)
  - FK opcional para `cadastro` com `ON DELETE SET NULL`
- Trigger `trg_cadastro_log` para registrar operações na tabela de log
- Usuário `app_user` com permissões apenas sobre `cadastro`

## Notas de Implementação

- O filtro de busca aceita:
  - Número: filtra por `idade = valor`
  - Texto: filtra `nome` por aproximação, usando `lower(nome) LIKE lower(@nome)`.
- A aplicação remove a função `unaccent` da query caso a extensão não esteja instalada, garantindo funcionamento mesmo sem a extensão.
- Ao salvar:
  - Se `Idade` estiver vazia, o valor enviado é `NULL`.

## Autor

- Luciano jr (LJCode)

## Licença

Este projeto pode ser utilizado para fins educativos e de demonstração. Adapte uma licença formal (ex.: MIT) conforme sua necessidade.

