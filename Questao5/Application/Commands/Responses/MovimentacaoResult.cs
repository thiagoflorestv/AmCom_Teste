using Questao5.Domain.Entities;

namespace Questao5.Application.Commands.Responses
{
    public class MovimentacaoResult
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public string? ErrorType { get; }
        public string IdMovimento { get; }

        public MovimentacaoResult(bool isSuccess, string message, string? errorType, string idMovimento)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorType = errorType;
            IdMovimento = idMovimento;
        }
                
    }

}
