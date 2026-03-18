using Dapper;
using Microsoft.AspNetCore.Mvc;
using Questao5.Infrastructure.Sqlite;
using System.Text.Json;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("movimentacao")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IDbConnectionFactory connectionFactory;

        public MovimentacaoController(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        [HttpPost]
        public IActionResult Movimentar([FromBody] MovimentoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdRequisicao))
            {
                return BadRequest(new ApiError("INVALID_VALUE", "A identificacao da requisicao deve ser informada."));
            }

            if (string.IsNullOrWhiteSpace(request.IdContaCorrente))
            {
                return BadRequest(new ApiError("INVALID_ACCOUNT", "A identificacao da conta corrente deve ser informada."));
            }

            request.TipoMovimento = request.TipoMovimento?.Trim().ToUpperInvariant() ?? string.Empty;

            if (request.Valor <= 0)
            {
                return BadRequest(new ApiError("INVALID_VALUE", "O valor deve ser maior que zero."));
            }

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
            {
                return BadRequest(new ApiError("INVALID_TYPE", "O tipo de movimento deve ser C ou D."));
            }

            using var connection = connectionFactory.CreateConnection();
            connection.Open();

            var idempotentResponse = connection.QueryFirstOrDefault<string>(
                "SELECT resultado FROM idempotencia WHERE chave_idempotencia = @chave;",
                new { chave = request.IdRequisicao });

            if (!string.IsNullOrWhiteSpace(idempotentResponse))
            {
                var resultFromCache = JsonSerializer.Deserialize<MovimentoResponse>(idempotentResponse);
                if (resultFromCache is not null)
                {
                    return Ok(resultFromCache);
                }
            }

            var conta = connection.QueryFirstOrDefault<ContaCorrenteDb>(
                "SELECT idcontacorrente, numero, nome, ativo FROM contacorrente WHERE idcontacorrente = @id;",
                new { id = request.IdContaCorrente });

            if (conta is null)
            {
                return BadRequest(new ApiError("INVALID_ACCOUNT", "Conta corrente nao encontrada."));
            }

            if (conta.Ativo != 1)
            {
                return BadRequest(new ApiError("INACTIVE_ACCOUNT", "Conta corrente inativa."));
            }

            var idMovimento = Guid.NewGuid().ToString().ToUpperInvariant();
            var dataMovimento = DateTime.UtcNow.ToString("O");

            using var transaction = connection.BeginTransaction();

            connection.Execute(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) " +
                "VALUES (@idmovimento, @idcontacorrente, @datamovimento, @tipomovimento, @valor);",
                new
                {
                    idmovimento = idMovimento,
                    idcontacorrente = request.IdContaCorrente,
                    datamovimento = dataMovimento,
                    tipomovimento = request.TipoMovimento,
                    valor = request.Valor
                },
                transaction);

            var result = new MovimentoResponse { IdMovimento = idMovimento };
            var serializedResult = JsonSerializer.Serialize(result);
            var serializedRequest = JsonSerializer.Serialize(request);

            connection.Execute(
                "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@chave, @requisicao, @resultado);",
                new
                {
                    chave = request.IdRequisicao,
                    requisicao = serializedRequest,
                    resultado = serializedResult
                },
                transaction);

            transaction.Commit();
            return Ok(result);
        }
    }

    [ApiController]
    [Route("saldo")]
    public class SaldoController : ControllerBase
    {
        private readonly IDbConnectionFactory connectionFactory;

        public SaldoController(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        [HttpGet("{idContaCorrente}")]
        public IActionResult ConsultarSaldo(string idContaCorrente)
        {
            if (string.IsNullOrWhiteSpace(idContaCorrente))
            {
                return BadRequest(new ApiError("INVALID_ACCOUNT", "A identificacao da conta corrente deve ser informada."));
            }

            using var connection = connectionFactory.CreateConnection();
            connection.Open();

            var conta = connection.QueryFirstOrDefault<ContaCorrenteDb>(
                "SELECT idcontacorrente, numero, nome, ativo FROM contacorrente WHERE idcontacorrente = @id;",
                new { id = idContaCorrente });

            if (conta is null)
            {
                return BadRequest(new ApiError("INVALID_ACCOUNT", "Conta corrente nao encontrada."));
            }

            if (conta.Ativo != 1)
            {
                return BadRequest(new ApiError("INACTIVE_ACCOUNT", "Conta corrente inativa."));
            }

            var saldo = connection.ExecuteScalar<decimal>(
                "SELECT COALESCE(SUM(CASE tipomovimento WHEN 'C' THEN valor WHEN 'D' THEN -valor END), 0) " +
                "FROM movimento WHERE idcontacorrente = @id;",
                new { id = idContaCorrente });

            return Ok(new SaldoResponse
            {
                Numero = conta.Numero,
                Nome = conta.Nome,
                DataHoraConsulta = DateTime.UtcNow,
                Saldo = saldo
            });
        }
    }

    public class MovimentoRequest
    {
        public string IdRequisicao { get; set; } = string.Empty;
        public string IdContaCorrente { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
    }

    public class MovimentoResponse
    {
        public string IdMovimento { get; set; } = string.Empty;
    }

    public class SaldoResponse
    {
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public DateTime DataHoraConsulta { get; set; }
        public decimal Saldo { get; set; }
    }

    public class ApiError
    {
        public ApiError(string tipo, string mensagem)
        {
            Tipo = tipo;
            Mensagem = mensagem;
        }

        public string Tipo { get; }
        public string Mensagem { get; }
    }

    public class ContaCorrenteDb
    {
        public string Idcontacorrente { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Ativo { get; set; }
    }
}
