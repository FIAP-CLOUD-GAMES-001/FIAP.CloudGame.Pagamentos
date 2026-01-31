# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER root
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

# Publica o projeto
RUN dotnet publish "FIAP.CloudGames.Pagamentos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
USER root

# Configurando timezone para America/Sao_Paulo
RUN apk add --no-cache tzdata icu-libs \
    && cp /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime \
    && echo "America/Sao_Paulo" > /etc/timezone \
    && apk del tzdata

ENV TZ=America/Sao_Paulo
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
COPY --from=build /app/publish .

RUN chown -R $APP_UID /app

EXPOSE 8080
EXPOSE 8081

USER $APP_UID
ENTRYPOINT ["dotnet", "FIAP.CloudGames.Pagamentos.Api.dll"]