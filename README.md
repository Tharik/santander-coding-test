# Hacker News – Best N Stories API

## Overview
This project implements an ASP.NET Core Web API that returns the top N best stories from the Hacker News API.
The API retrieves story IDs from the official Hacker News endpoint and then fetches the corresponding story details. The results are ordered by score in descending order and the top N stories are returned.

### How to Run the Application

#### Requirements
```
.NET 8 SDK (or compatible version)
```

#### Restore dependencies
```bash
cd BestStoriesApi/
dotnet restore
```

#### Run the application
```bash
dotnet run
```

The API will start and expose:
```code
GET /api/v1/beststories?n=10
```

#### How to Test the API
There are two simple ways to test the endpoint after running the application:

#### 1️⃣ Using curl:

You can test directly from the terminal:
```bash
curl -X GET "http://localhost:5136/api/v1/beststories?n=20" \
     -H "Accept: application/json"
```

You should receive a JSON array containing the top 20 best stories ordered by score (descending).


#### 2️⃣ Using the .http file (VS Code / Rider)
This project includes an HTTP file for quick manual testing so basically just press the *Send Request* button in the **BestStoriesApi.http** file

### Assumptions Made
 - The maximum value for n is limited to 500;
 - If Hacker News fails to return valid story data, the API returns an error instead of incomplete results;
 - Only items with type = story are considered valid;
 - Scores may not reflect real-time values due to caching.

 ### Enhancements
 - Use distributed cache (e.g., Redis) for multi-instance scenarios;
 - Add retry and circuit breaker policies;
 - Improve Swagger/OpenAPI documentation;
 - Add authentication and basic security protections;
 - Implement rate limiting (throttling);
 - Add structured logging and monitoring;
 - Provide a small CLI and/or simple UI;
 - Add unit and integration tests.

# 🇵🇹 Versão em Português

## Visão Geral
Este projeto implementa uma API em ASP.NET Core que retorna as N melhores histórias do Hacker News.
A API busca os IDs das melhores histórias e depois consulta os detalhes de cada uma. O resultado é ordenado por score em ordem decrescente e retorna as N melhores.

## Como Executar a Aplicação
### Requisitos
```
.NET 8 SDK (ou versão compatível)
```

### Restaurar dependências
```bash
cd BestStoriesApi/
dotnet restore
```

### Executar a aplicação
```bash
dotnet run
```

### Endpoint disponível
```code
GET /api/v1/beststories?n=10
```

### Como testar a API

Existem duas formas simples de testar o endpoint após executar a aplicação:

### 1️⃣ Usando curl
No terminal:
```bash
curl -X GET "http://localhost:5136/api/v1/beststories?n=20" \
     -H "Accept: application/json"
```

A resposta será um array JSON com as 20 melhores histórias ordenadas por score (decrescente).


### 2️⃣ Usando o arquivo .http
O projeto inclui um arquivo .http para testes rápidos. Basta abrir o 
arquivo **BestStoriesApi.http** e pressionar o botão *Send Request*


### Premissas Consideradas
 - O valor máximo de n é 500;
 - Caso o Hacker News falhe ao retornar dados válidos, a API retorna erro ao invés de dados incompletos;
 - Apenas itens com type = story são considerados válidos;
 - O score pode não refletir valores em tempo real devido ao cache.

### Melhorias Futuras
 - Utilizar cache distribuído (ex.: Redis);
 - Implementar retry e circuit breaker;
 - Melhorar a documentação Swagger/OpenAPI;
 - Adicionar autenticação e proteções básicas de segurança;
 - Implementar rate limiting (throttling);
 - Adicionar logs estruturados e monitoramento;
 - Criar uma CLI simples e/ou uma UI;
 - Adicionar testes unitários e de integração.