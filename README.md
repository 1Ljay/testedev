# TesteDev.WinForms

Aplicativo Windows Forms (.NET 8) com CRUD simples em PostgreSQL. A aplicação permite cadastrar pessoas com `nome` e `idade`, realizando operações de inserir, atualizar, excluir e listar com filtro.

## Tecnologias

- C# / .NET 8 (Windows Forms)
- PostgreSQL 13+
- Npgsql (driver PostgreSQL para .NET)

## Requisitos

- Windows 10+
- PostgreSQL 13+ instalado e em execução
- Usuário do banco com permissão de criação (para rodar `testedev.sql`)

> **Nota:** Para desenvolvimento, é necessário o [.NET SDK 8.x](https://dotnet.microsoft.com/). Para instalação via setup, não é necessário, pois o instalador já inclui o runtime necessário.

## Instalação e Execução

### Opção 1: Instalação via Setup (Recomendado)

1. Navegue até a pasta `publish` na raiz do projeto.
2. Execute o arquivo `setup.exe`.
3. Siga o assistente de instalação para instalar o aplicativo.
4. Após a instalação, o aplicativo estará disponível no menu Iniciar do Windows e poderá ser executado normalmente.

> **Importante:** Antes de executar o aplicativo pela primeira vez, certifique-se de que o banco de dados PostgreSQL está configurado (veja a seção "Configuração do Banco de Dados" abaixo).

### Opção 2: Execução via Visual Studio (Desenvolvimento)

1. Abra o repositório no Visual Studio 2022+ ou no VS Code (com extensões C#).
2. Garanta que o banco `TesteDev` esteja com o schema criado (rodar `testedev.sql`).
3. Execute o projeto `TesteDev.WinForms` (F5). A janela principal (`MainForm`) abrirá.

## Configuração do Banco de Dados

1. Abra o pgAdmin e crie (se necessário) o banco `TesteDev`.
2. No pgAdmin, restaure o banco de dados usando o arquivo `testedev.sql` localizado na raiz do projeto.
   - O arquivo contém o backup completo do banco com as tabelas `public.cadastro` e `public.log_operacoes`, a função/trigger de log, e o usuário `app_user` com as permissões adequadas.
3. O `testedev.sql` também inclui dados iniciais para teste.

Caso queira alterar o usuário/senha do banco, ajuste a connection string:

- Arquivo: `TesteDev.WinForms/appsettings.json`
- Chave: `ConnectionStrings.Pg`

Exemplo padrão utilizado pela aplicação:

```
Host=localhost;Port=5432;Database=TesteDev;Username=app_user;Password=SenhaForte!123;Pooling=true;
```

Se o `appsettings.json` não for encontrado no diretório de execução, a aplicação usará a mesma connection string acima como fallback.

## Como Usar a Aplicação

Após instalar e executar o aplicativo, utilize os seguintes campos e funcionalidades:

- **Nome** (texto): Campo obrigatório para o nome da pessoa
- **Idade** (numérica): Campo opcional, aceita apenas valores positivos. Se deixado vazio, será enviado como NULL
- **Botões:**
  - `Novo`: Limpa os campos para cadastrar uma nova pessoa
  - `Salvar`: Salva ou atualiza o registro selecionado
  - `Excluir`: Remove o registro selecionado
- **Buscar**: Campo e botão para filtrar registros
  - Se digitar um número: filtra por idade exata
  - Se digitar texto: filtra por nome usando aproximação (sem acentos se a extensão unaccent estiver disponível; a aplicação faz fallback caso não esteja)

## Estrutura do Projeto

```
TesteDev.WinForms.sln                  # Solução
TesteDev.WinForms/                     # Projeto WinForms
  Program.cs                           # Inicialização e leitura de appsettings.json
  MainForm.cs                          # Lógica da tela, CRUD e validações
  MainForm.Designer.cs                 # Layout da tela
  appsettings.json                     # Connection string (opcional em runtime)
publish/                                # Pasta com instalador ClickOnce
  setup.exe                            # Instalador do aplicativo
  TesteDev.WinForms.application        # Manifesto da aplicação
testedev.sql                            # Backup completo do banco de dados PostgreSQL
```

## Estrutura do Banco de Dados

### Tabelas

- **`public.cadastro`**

  - `id` (IDENTITY, PK) - Identificador único
  - `nome` (TEXT, NOT NULL) - Nome da pessoa (com constraint `ck_nome_not_blank`)
  - `idade` (INTEGER, NULL) - Idade da pessoa
    - Constraint `ck_idade_pos`: assegura `idade > 0` quando informado
    - Constraint `uq_idade`: impede repetição de idade
  - Índice `idx_cadastro_nome` em `lower(nome)` para acelerar buscas por nome

- **`public.log_operacoes`**
  - Log automático de `INSERT`, `UPDATE`, `DELETE` em `cadastro`
  - `instante` (TIMESTAMPTZ) - Data e hora da operação
  - FK opcional para `cadastro` com `ON DELETE SET NULL`
  - Trigger `trg_cadastro_log` registra automaticamente todas as operações

### Usuário do Banco

- Usuário `app_user` criado pelo backup `testedev.sql` com permissões apenas sobre a tabela `cadastro`

## Notas de Implementação

- **Sistema de Busca:**

  - Se o filtro for um número: busca por idade exata (`idade = valor`)
  - Se o filtro for texto: busca por nome usando aproximação (`lower(nome) LIKE lower(@nome)`)
  - A aplicação tenta usar a função `unaccent` para busca sem acentos, mas faz fallback caso a extensão não esteja instalada

- **Validações:**
  - Nome é obrigatório e não pode estar em branco
  - Idade deve ser positiva quando informada
  - Se `Idade` estiver vazia ao salvar, o valor enviado é `NULL`

## Autor

- Luciano Jr

## Licença

Este projeto pode ser utilizado para fins educativos e de demonstração. Adapte uma licença formal (ex.: MIT) conforme sua necessidade.
