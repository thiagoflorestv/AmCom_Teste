namespace Questao5.Application.Commands.Requests
{
    public class MovimentarContaRequest
    {
        public string? IdReq { get; set; }
        public string? IdContaCorrente { get; set; }
        public decimal Valor { get; set; }
        public string? TipoMovimento { get; set; }
    }

}
