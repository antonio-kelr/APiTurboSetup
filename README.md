# APiTurboSetup - Loja Virtual

## Descrição
API desenvolvida em .NET para gerenciar uma loja virtual, incluindo funcionalidades de cadastro de usuários, autenticação (Google e Firebase), gerenciamento de produtos, categorias, carrinho de compras, pedidos, favoritos, endereços, entre outros.

---

## Pré-requisitos
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/)
- Banco de dados PostgreSQL (pode ser rodado via Docker)

---

## Como rodar localmente (sem Docker)
1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   cd APiTurboSetup
   ```
2. **Configure o arquivo de ambiente:**
   - Copie o arquivo `appsettings.example.json` para `appsettings.Development.json` e ajuste as configurações (principalmente a string de conexão do banco de dados e credenciais de serviços externos).

3. **Restaure os pacotes:**
   ```bash
   dotnet restore
   ```
4. **Aplique as migrations (opcional):**
   ```bash
   dotnet ef database update
   ```
5. **Rode a aplicação:**
   ```bash
   dotnet run
   ```
   A API estará disponível em `https://localhost:5001` ou `http://localhost:5000`.

---

## Como rodar com Docker
1. **Configure o arquivo de ambiente:**
   - Ajuste o arquivo `appsettings.Production.json` conforme necessário.
2. **Suba os containers:**
   ```bash
   docker-compose up --build
   ```
   Isso irá subir a API e o banco de dados PostgreSQL já configurado.

---

## Estrutura de Pastas
- `Controllers/` - Controllers das rotas da API
- `Models/` - Modelos das entidades do sistema
- `Repositories/` - Implementação dos repositórios (acesso ao banco)
- `Interfaces/` - Interfaces dos repositórios e serviços
- `Services/` - Serviços auxiliares (e.g., envio de e-mail, autenticação)
- `Data/` - Contexto do banco de dados (Entity Framework)
- `Migrations/` - Migrations do banco de dados
- `Utils/` - Utilitários diversos
- `Validations/` - Validações customizadas

---

## Principais Rotas da API

### Usuários
- `POST /api/users/register` - Cadastro de usuário
- `POST /api/users/login` - Login de usuário
- `POST /api/users/troca-email` - Solicitar troca de e-mail
- `POST /api/users/troca-senha` - Solicitar troca de senha

### Produtos
- `GET /api/produtos` - Listar produtos
- `GET /api/produtos/{id}` - Detalhes de um produto
- `POST /api/produtos` - Criar produto
- `PUT /api/produtos/{id}` - Atualizar produto
- `DELETE /api/produtos/{id}` - Remover produto

### Categorias
- `GET /api/categorias` - Listar categorias
- `POST /api/categorias` - Criar categoria

### Carrinho
- `GET /api/carrinho` - Visualizar carrinho
- `POST /api/carrinho` - Adicionar item ao carrinho
- `DELETE /api/carrinho/{id}` - Remover item do carrinho

### Favoritos
- `GET /api/favorito` - Listar favoritos
- `POST /api/favorito` - Adicionar aos favoritos
- `DELETE /api/favorito/{id}` - Remover dos favoritos

### Pedidos
- `GET /api/pedido` - Listar pedidos do usuário
- `POST /api/pedido` - Criar novo pedido

### Endereços
- `GET /api/endereco` - Listar endereços
- `POST /api/endereco` - Adicionar endereço
- `PUT /api/endereco/{id}` - Atualizar endereço
- `DELETE /api/endereco/{id}` - Remover endereço

### Autenticação Google
- `POST /api/googleauth/login` - Login com Google

### Imagens de Produto
- `POST /api/produtoimagens/upload` - Upload de imagem de produto
- `GET /api/produtoimagens/{produtoId}` - Listar imagens de um produto

---

## Configuração de Variáveis/Ambiente
- `ConnectionStrings:DefaultConnection` - String de conexão com o banco de dados
- `Firebase` - Configurações do Firebase (arquivo JSON)
- `Jwt:Key` - Chave secreta para geração de tokens JWT
- Outras variáveis podem ser necessárias conforme integrações (e-mail, storage, etc.)

---

## Observações
- Para rodar migrations, utilize o comando `dotnet ef migrations add <NomeDaMigration>`
- O projeto utiliza Entity Framework Core para ORM
- Para testes de rotas, utilize ferramentas como [Postman](https://www.postman.com/) ou [Insomnia](https://insomnia.rest/)
- O Docker Compose já está configurado para rodar a API e o banco de dados juntos

---

## Contato
Dúvidas ou sugestões? Entre em contato com o mantenedor do projeto. 