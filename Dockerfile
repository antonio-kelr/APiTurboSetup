# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura dependências
COPY *.csproj .
RUN dotnet restore

# Copia todo o código e publica a aplicação
COPY . .
RUN dotnet publish -c Release -o out

# Etapa final: imagem runtime mais leve
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expõe a porta que seu backend escuta (ajuste se necessário)
EXPOSE 5299

# Comando para rodar sua aplicação
ENTRYPOINT ["dotnet", "APiTurboSetup.dll"]
