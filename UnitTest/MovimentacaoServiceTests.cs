using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Questao5;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Services;
using Questao5.Infrastructure.Services.Controllers;
using System.Data;
using System.Data.Common;
using Xunit;

namespace UnitTest
{
    public class MovimentacaoServiceTests : IDisposable
    {
        private readonly IDbConnection _dbConnection;
        private readonly MovimentacaoService _movimentacaoService;

        public MovimentacaoServiceTests()
        {
            // Configuração da conexão SQLite em memória
            _dbConnection = new SqliteConnection("DataSource=:memory:");
            _dbConnection.Open();

            // Criação das tabelas
            var createTableContaCorrente = @"CREATE TABLE contacorrente (
                                                idcontacorrente TEXT PRIMARY KEY,
                                                numero INTEGER NOT NULL UNIQUE,
                                                nome TEXT NOT NULL,
                                                ativo INTEGER NOT NULL CHECK (ativo IN (0,1))
                                              )";
            var createTableMovimento = @"CREATE TABLE movimento (
                                            idmovimento TEXT PRIMARY KEY,
                                            idcontacorrente INTEGER NOT NULL,
                                            datamovimento TEXT NOT NULL,
                                            tipomovimento TEXT NOT NULL CHECK (tipomovimento IN ('C', 'D')),
                                            valor REAL NOT NULL
                                          )";

            _dbConnection.Execute(createTableContaCorrente);
            _dbConnection.Execute(createTableMovimento);

            // Inicializando o serviço
            _movimentacaoService = new MovimentacaoService(_dbConnection);
        }

        [Fact]
        public async Task MovimentarContaCorrente_DeveCriarMovimento_QuandoDadosValidos()
        {            
            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                Numero = 123,
                Nome = "Katherine Sanchez",
                Ativo = true // Conta ativa
            };

            // Inserindo a conta corrente
            await _dbConnection.ExecuteAsync("INSERT INTO contacorrente (idcontacorrente, numero, nome, ativo) VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo)", contaCorrente);

            var request = new MovimentarContaRequest
            {
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                Valor = 100.0m,
                TipoMovimento = "C"
            };
                        
            var idMovimento = await _movimentacaoService.MovimentarContaCorrenteAsync(request);
                        
            var movimento = await _dbConnection.QueryFirstOrDefaultAsync<Movimento>(
                "SELECT * FROM movimento WHERE idmovimento = @IdMovimento", new { IdMovimento = idMovimento });

            movimento.Should().NotBeNull();
            movimento.Valor.Should().Be(100.0m);
            movimento.TipoMovimento.Should().Be("C");
        }

        [Fact]
        public async Task MovimentarContaCorrente_DeveRetornarErro_QuandoContaInativa()
        {
            // Arrange
            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = "F475F943-7067-ED11-A06B-7E5DFA4A16C9",
                Numero = 741,
                Nome = "Ameena Lynn",
                Ativo = false // Conta inativa
            };

            // Inserindo a conta corrente
            await _dbConnection.ExecuteAsync("INSERT INTO contacorrente (idcontacorrente, numero, nome, ativo) VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo)", contaCorrente);

            var request = new MovimentarContaRequest
            {
                IdContaCorrente = "F475F943-7067-ED11-A06B-7E5DFA4A16C9",
                Valor = 100.0m,
                TipoMovimento = "C"
            };
                        
            Func<Task> act = async () => await _movimentacaoService.MovimentarContaCorrenteAsync(request);
                        
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Conta inativa");
        }

        [Fact]
        public async Task ConsultarSaldo_DeveRetornarSaldoCorreto_QuandoDadosValidos()
        {            
            var contaCorrente = new ContaCorrente
            {
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                Numero = 123,
                Nome = "Katherine Sanchez",
                Ativo = true // Conta ativa
            };

            // Inserindo a conta corrente
            await _dbConnection.ExecuteAsync("INSERT INTO contacorrente (idcontacorrente, numero, nome, ativo) VALUES (@IdContaCorrente, @Numero, @Nome, @Ativo)", contaCorrente);

            // Adicionando movimentos
            await _dbConnection.ExecuteAsync(
                "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)",
                new[]
                {
                    new Movimento { IdMovimento = Guid.NewGuid().ToString(), IdContaCorrente = 123, TipoMovimento = "C", Valor = 200.0m, DataMovimento = DateTime.Now.ToString("dd/MM/yyyy") },
                    new Movimento { IdMovimento = Guid.NewGuid().ToString(), IdContaCorrente = 123, TipoMovimento = "D", Valor = 50.0m, DataMovimento = DateTime.Now.ToString("dd/MM/yyyy") }
                });
                        
            var saldo = await _movimentacaoService.ConsultarSaldoAsync("B6BAFC09-6967-ED11-A567-055DFA4A16C9");
                        
            saldo.Should().Be(150.0m);
        }

        [Fact]
        public async Task ConsultarSaldo_DeveRetornarErro_QuandoContaNaoEncontrada()
        {            
            Func<Task> act = async () => await _movimentacaoService.ConsultarSaldoAsync("INVALID_ACCOUNT");
                        
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Conta não encontrada");
        }

        // Limpeza após os testes
        public void Dispose()
        {
            _dbConnection.Close();
        }
    }
}
