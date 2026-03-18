# technical-challenge-dotnet

Resolução de desafio técnico em .NET, organizada por questão.

## Estrutura do repositório

- `Questao1`: aplicação de console com modelagem de conta bancária e regras de depósito/saque.
- `Questao2`: aplicação de console que consome API de partidas de futebol e calcula total de gols por time/ano.
- `Questao3`: resolução em documento (`.docx`).
- `Questao4`: resolução em documento (`.docx`).
- `Questao5`: API REST em ASP.NET Core com SQLite/Dapper para movimentação e consulta de saldo.

## Tecnologias utilizadas

- .NET 6
- ASP.NET Core Web API
- SQLite
- Dapper
- Newtonsoft.Json
- Swagger (Swashbuckle)

## Pré-requisitos

- SDK do .NET 6 ou superior instalado.

## Como executar

No diretório raiz do repositório:

```powershell
dotnet build .\Exercicio.sln
```

### Questao1

```powershell
dotnet run --project .\Questao1\Questao1.csproj
```

### Questao2

```powershell
dotnet run --project .\Questao2\Questao2.csproj
```

### Questao5

```powershell
dotnet run --project .\Questao5\Questao5.csproj
```

Com a API em execução, acessar Swagger:

- `https://localhost:7140/swagger`
- ou `http://localhost:5189/swagger`

## Questão 5 - Endpoints implementados

- `POST /movimentacao`
  - Recebe: identificação da requisição, identificação da conta corrente, valor e tipo de movimento (`C`/`D`).
  - Validações: conta existente, conta ativa, valor positivo e tipo válido.
  - Retorna `idMovimento` em caso de sucesso.

- `GET /saldo/{idContaCorrente}`
  - Retorna: número da conta, nome do titular, data/hora da consulta e saldo atual.
  - Saldo calculado por: soma dos créditos menos soma dos débitos.

## Observações

- As tabelas do SQLite são criadas automaticamente na inicialização da `Questao5`.
- A carga inicial de contas correntes é feita automaticamente no bootstrap do banco.
- As questões 3 e 4 estão apresentadas em seus respectivos documentos na pasta de cada questão.
