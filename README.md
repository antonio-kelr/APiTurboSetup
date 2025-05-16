# API Turbo Setup

## Configuração do Projeto

### Arquivos de Configuração

1. Copie o arquivo `appsettings.example.json` para `appsettings.json`:
   ```bash
   cp appsettings.example.json appsettings.json
   ```

2. Configure as seguintes informações no `appsettings.json`:
   - ConnectionStrings: Configure sua string de conexão com o banco de dados
   - Jwt: Configure suas chaves JWT
   - Firebase: Configure suas credenciais do Firebase

3. Adicione seu arquivo `firebase-adminsdk.json` na raiz do projeto

### Arquivos Ignorados pelo Git

Os seguintes arquivos são ignorados pelo Git por conterem informações sensíveis:
- `appsettings.json`
- `firebase-adminsdk.json`
- Arquivos `.env`
- Arquivos de certificados (`.pfx`, `.key`)

### Segurança

NUNCA comite os seguintes arquivos no GitHub:
- `appsettings.json` (use o `appsettings.example.json` como template)
- `firebase-adminsdk.json`
- Qualquer arquivo contendo senhas ou chaves secretas

### Desenvolvimento

1. Clone o repositório
2. Copie o `appsettings.example.json` para `appsettings.json`
3. Configure suas credenciais no `appsettings.json`
4. Adicione seu arquivo `firebase-adminsdk.json`
5. Execute as migrações do banco de dados:
   ```bash
   dotnet ef database update
   ```
6. Execute o projeto:
   ```bash
   dotnet run
   ``` 