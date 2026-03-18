# technical-challenge-dotnet

Repositório com minha resolução do desafio técnico em .NET.

## Organização

- `Questao1`: console app com classe de conta bancária.
- `Questao2`: console app que consulta API e soma gols por time/ano.
- `Questao3`: resposta em documento (`.docx`).
- `Questao4`: resposta em documento (`.docx`).
- `Questao5`: API REST para movimentação e consulta de saldo.

## Stack usada

- .NET 6
- ASP.NET Core
- SQLite
- Dapper
- Newtonsoft.Json
- Swagger

## Como rodar

Na pasta raiz:

```powershell
dotnet build .\Exercicio.sln
```

Executar cada projeto:

```powershell
dotnet run --project .\Questao1\Questao1.csproj
dotnet run --project .\Questao2\Questao2.csproj
dotnet run --project .\Questao5\Questao5.csproj
```

Swagger da `Questao5`:

- `https://localhost:7140/swagger`
- `http://localhost:5189/swagger`

## Questao5 - endpoints

- `POST /movimentacao`
  - recebe `idRequisicao`, `idContaCorrente`, `valor` e `tipoMovimento` (`C` ou `D`)
  - valida conta, status da conta, valor e tipo
  - retorna `idMovimento` quando sucesso

- `GET /saldo/{idContaCorrente}`
  - retorna numero da conta, nome do titular, data/hora e saldo atual
  - saldo calculado por creditos - debitos

## Observações

- O banco da `Questao5` é criado automaticamente ao iniciar a API.
- As contas iniciais também são carregadas no bootstrap.
