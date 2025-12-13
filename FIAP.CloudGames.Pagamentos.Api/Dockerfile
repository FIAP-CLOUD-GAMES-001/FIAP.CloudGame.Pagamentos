# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia a solution e os arquivos .csproj
COPY ["FIAP.CloudGames.Pagamentos.sln", "."]
COPY ["FIAP.CloudGames.Pagamentos.Api/FIAP.CloudGames.Pagamentos.Api.csproj", "FIAP.CloudGames.Pagamentos.Api/"]
COPY ["FIAP.CloudGames.Pagamentos.Domain/FIAP.CloudGames.Pagamentos.Domain.csproj", "FIAP.CloudGames.Pagamentos.Domain/"]
COPY ["FIAP.CloudGames.Pagamentos.Infrastructure/FIAP.CloudGames.Pagamentos.Infrastructure.csproj", "FIAP.CloudGames.Pagamentos.Infrastructure/"]
COPY ["FIAP.CloudGames.Pagamentos.Service/FIAP.CloudGames.Pagamentos.Service.csproj", "FIAP.CloudGames.Pagamentos.Service/"]

# Restaura as dependências
RUN dotnet restore "FIAP.CloudGames.Pagamentos.sln"

# Copia todo o código fonte
COPY . .

# Compila o projeto
WORKDIR "/src/FIAP.CloudGames.Pagamentos.Api"
RUN dotnet build "FIAP.CloudGames.Pagamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FIAP.CloudGames.Pagamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FIAP.CloudGames.Pagamentos.Api.dll"]