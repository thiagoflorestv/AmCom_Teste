using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Infrastructure.Services.Interfaces;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovimentacaoController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        public MovimentacaoController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService;
        }

        /// <summary>
        /// Realiza a movimentação de uma conta corrente (crédito ou débito).
        /// </summary>
        /// <param name="idReq">Identificação única da requisição (idempotente).</param>
        /// <param name="idContaCorrente">Identificação da conta corrente.</param>
        /// <param name="valor">Valor da movimentação (positivo).</param>
        /// <param name="tipoMovimento">Tipo de movimento: C (Crédito) ou D (Débito).</param>
        /// <returns>Retorna um código HTTP 200 com o ID do movimento gerado ou um erro HTTP 400 com a descrição do erro.</returns>
        [HttpPost("movimentar")]
        public async Task<IActionResult> MovimentarContaCorrenteAsync([FromBody] MovimentarContaRequest request)
        {
            var result = await _movimentacaoService.MovimentarContaCorrenteAsync(request);
            if (result.IsSuccess)
            {
                return Ok(new { IdMovimento = result.IdMovimento });
            }
            return BadRequest(new { mensagem = result.Message, tipo = result.ErrorType });
        }

        /// <summary>
        /// Consulta o saldo de uma conta corrente.
        /// </summary>
        /// <param name="idContaCorrente">Identificação da conta corrente.</param>
        /// <returns>Retorna o saldo da conta corrente ou um erro HTTP 400 em caso de falha.</returns>
        [HttpGet("saldo/{idContaCorrente}")]
        public async Task<IActionResult> ConsultarSaldoAsync(string idContaCorrente)
        {
            var result = await _movimentacaoService.ConsultarSaldoAsync(idContaCorrente);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    NumeroConta = result.Conta?.Numero,
                    NomeTitular = result.Conta?.Nome,
                    DataConsulta = DateTime.Now,
                    result.Saldo
                });
            }
            return BadRequest(new { mensagem = result.Message, tipo = result.ErrorType });
        }
    }

}
