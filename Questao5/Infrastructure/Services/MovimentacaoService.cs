using Dapper;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Services.Interfaces;
using System.Data;

namespace Questao5.Infrastructure.Services
{
    public class MovimentacaoService : IMovimentacaoService
    {
        private readonly IDbConnection _dbConnection;

        public MovimentacaoService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<MovimentacaoResult> MovimentarContaCorrenteAsync(MovimentarContaRequest request)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { request.IdContaCorrente });

            if (conta == null)
                return new MovimentacaoResult(false, "Conta não encontrada", null, "INVALID_ACCOUNT");

            if (!conta.Ativo)
                return new MovimentacaoResult(false, "Conta inativa", null, "INACTIVE_ACCOUNT");

            if (request.Valor <= 0)
                return new MovimentacaoResult(false, "Valor inválido", null, "INVALID_VALUE");

            if (request.TipoMovimento != "C" && request.TipoMovimento != "D")
                return new MovimentacaoResult(false, "Tipo de movimento inválido", null, "INVALID_TYPE");

            var movimentoId = Guid.NewGuid().ToString();
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new { IdMovimento = movimentoId, request.IdContaCorrente, DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"), request.TipoMovimento, request.Valor });

            return new MovimentacaoResult(true, "Movimentação realizada com sucesso", null, movimentoId);
        }

        public async Task<SaldoResult> ConsultarSaldoAsync(string idContaCorrente)
        {
            var conta = await _dbConnection.QueryFirstOrDefaultAsync<ContaCorrente>(
                "SELECT * FROM contacorrente WHERE idcontacorrente = @IdContaCorrente",
                new { idContaCorrente });

            if (conta == null)
                return new SaldoResult(false, "Conta não encontrada", "INVALID_ACCOUNT");

            if (!conta.Ativo)
                return new SaldoResult(false, "Conta inativa", "INACTIVE_ACCOUNT");

            var creditos = await _dbConnection.QueryAsync<Movimento>(
                "SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'C'",
                new { idContaCorrente });

            var debitos = await _dbConnection.QueryAsync<Movimento>(
                "SELECT SUM(valor) FROM movimento WHERE idcontacorrente = @IdContaCorrente AND tipomovimento = 'D'",
                new { idContaCorrente });

            decimal saldo = creditos.Sum(c => c.Valor) - debitos.Sum(d => d.Valor);

            return new SaldoResult(true, saldo);
        }
    }
}
