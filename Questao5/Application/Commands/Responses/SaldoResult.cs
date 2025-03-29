using Questao5.Domain.Entities;

namespace Questao5.Application.Commands.Responses
{
    public class SaldoResult
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public string? ErrorType { get; }
        public decimal Saldo { get; }
        public ContaCorrente? Conta { get; }

        public SaldoResult(bool isSuccess, decimal saldo)
        {
            IsSuccess = isSuccess;
            Saldo = saldo;
        }

        public SaldoResult(bool isSuccess, string message, string errorType)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorType = errorType;
        }
    }
}
