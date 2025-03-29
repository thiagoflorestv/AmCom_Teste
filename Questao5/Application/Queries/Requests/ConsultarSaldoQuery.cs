using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Questao5.Application.Queries.Requests
{
    public class ConsultarSaldoQuery : IRequest<IActionResult>
    {
        public int IdContaCorrente { get; set; }
    }
}
