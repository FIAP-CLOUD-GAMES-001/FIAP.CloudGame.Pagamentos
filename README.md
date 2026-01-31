# FIAP.CloudGame.Pagamentos

## 📋 Descrição

API .NET 8.0 responsável pelo processamento de pagamentos. Recebe requisições de pagamento via HTTP da Games API, persiste os dados no MongoDB e publica notificações de status de pagamento para consumo assíncrono.

## 🎯 Função Principal

A aplicação processa pagamentos através do endpoint `/api/payment` e realiza as seguintes operações:

1. **Recebe** requisições de pagamento via HTTP (OrderId, valor, método de pagamento, data)
2. **Processa** e **define** o status do pagamento
3. **Persiste** os dados no MongoDB
4. **Publica** notificação de status de pagamento para mensageria (RabbitMQ)

## 🏗️ Arquitetura

Aplicação estruturada em camadas:

- **Domain**: Entidades, interfaces e modelos de domínio
- **Service**: Lógica de negócio e processamento de pagamentos
- **Infrastructure**: Repositórios MongoDB, mensageria e configurações de dados
- **Api**: Controllers, middlewares e configurações da API

## 🛠️ Tecnologias

- **.NET 8.0**
- **MongoDB**
- **RabbitMQ**
- **JWT Authentication**
- **Serilog** (logging no MongoDB)
- **Swagger/OpenAPI**

## ⚙️ Configuração

Configure as seguintes propriedades no `appsettings.json`:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "cloudgames-payments"
  },
    "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "admin",
    "Password": "admin",
    "ExchangeName": "payment-exchange",
    "QueueName": "payment-success-queue",
    "RetryQueueName": "payment-retry-queue",
    "FailQueueName": "payment-fail-queue",
    "RoutingKey": "payment.notification"
  },
  "Jwt": {
    "Key": "...",
    "Issuer": "FIAP.CloudGames",
    "Audience": "FIAP.CloudGames"
  }
}
````

## 🚀 Execução

```bash
dotnet run --project FIAP.CloudGames.Pagamentos.Api
```

A API estará disponível na porta configurada e o Swagger em `/swagger`.

## 📡 Endpoints

* `POST /api/payment`
  Recebe uma solicitação de pagamento enviada pela Games API, processa e publica o status do pagamento.

* `GET /api/payment`
  Lista todos os pagamentos.

* `GET /api/payment/{orderId}`
  Busca pagamento pelo OrderId.

## 📦 Integração com Games API

* A **Games API** envia a solicitação de pagamento via **HTTP**
* A **Payments API** processa e publica o resultado do pagamento via **mensageria**
* A **Games API** consome a notificação para atualização do status do pedido