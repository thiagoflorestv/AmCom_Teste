using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;

namespace Questao5.Infrastructure.Services.Interfaces
{
    public interface IMovimentacaoService
    {
        Task<MovimentacaoResult> MovimentarContaCorrenteAsync(MovimentarContaRequest request);
        Task<SaldoResult> ConsultarSaldoAsync(string idContaCorrente);
    }
}
