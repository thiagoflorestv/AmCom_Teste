using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria {
        public int Numero { get; private set; }
        public string Titular { get; set; }
        public double Saldo { get; private set; }

        
        private const double TaxaSaque = 3.50;
                
        public ContaBancaria(int numero, string titular, double depositoInicial = 0.0)
        {
            Numero = numero;
            Titular = titular;
            Saldo = depositoInicial;
        }

        // Realizar Depósito
        public void Deposito(double valor)
        {
            if (valor > 0)
            {
                Saldo += valor;
            }
            else
            {
                Console.WriteLine("Valor de depósito inválido.");
            }
        }

        // Realizar Saque
        public void Saque(double valor)
        {
            if (valor + TaxaSaque <= Saldo)
            {
                Saldo -= valor + TaxaSaque;
            }
            else
            {
                Console.WriteLine("Saldo insuficiente para realizar o saque e pagar a taxa.");
            }
        }

        // Exibir os dados da Conta
        public void ExibirDados()
        {
            Console.WriteLine($"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo:F2}");
        }

    }
}
