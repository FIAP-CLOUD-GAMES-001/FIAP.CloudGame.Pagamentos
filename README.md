# FIAP.CloudGame.Pagamentos

## 📋 Descrição

API .NET 8.0 para processamento de pagamentos que recebe requisições de pagamento, persiste os dados no MongoDB e envia notificações para uma Azure Function.

## 🎯 Função Principal

A aplicação processa pagamentos através do endpoint `/api/payment` e realiza as seguintes operações:

1. **Recebe** requisições de pagamento (OrderId, valor, método de pagamento, data)
2. **Cria** e **aprova** o pagamento automaticamente
3. **Persiste** os dados no MongoDB
4. **Envia** notificação para Azure Function via webhook (`/api/webhook/payment`)

## 🏗️ Arquitetura

Aplicação estruturada em camadas:

- **Domain**: Entidades, interfaces e modelos de domínio
- **Service**: Lógica de negócio e processamento de pagamentos
- **Infrastructure**: Repositórios MongoDB e configurações de dados
- **Api**: Controllers, middlewares e configurações da API

## 🛠️ Tecnologias

- .NET 8.0
- MongoDB
- JWT Authentication
- Azure Functions (integração via HTTP)
- Serilog (logging no MongoDB)
- Swagger/OpenAPI

## ⚙️ Configuração

Configure as seguintes propriedades no `appsettings.json`:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "cloudgames-payments"
  },
  "AzureFunctions": {
    "BaseUrl": "http://localhost:7071",
    "FunctionKey": ""
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "FIAP.CloudGames",
    "Audience": "FIAP.CloudGames"
  }
}
```

## 🚀 Execução

```bash
dotnet run --project FIAP.CloudGames.Pagamentos.Api
```

A API estará disponível em `https://localhost:5001` (ou porta configurada) e o Swagger em `/swagger`.

## 📡 Endpoints

- `POST /api/payment` - Processa um pagamento e envia notificação para Azure Function
- `GET /api/payment` - Lista pagamentos
- `GET /api/payment/{orderId}` - Busca pagamento por OrderId