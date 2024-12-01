using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

namespace UnitTests.UseCases
{
    [Binding]
    public class GetPaymentStatusSteps
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly GetPaymentStatus _getPaymentStatus;
        private PaymentStatusDto? _result;

        public GetPaymentStatusSteps()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _getPaymentStatus = new GetPaymentStatus(_paymentRepositoryMock.Object);
        }

        [Given(@"que existe um repositório de pagamentos disponível")]
        public void DadoQueExisteUmRepositórioDePagamentosDisponível()
        {
            // Mock já criado no construtor.
        }

        [Given(@"que existe um pagamento aprovado com o OrderId (.*)")]
        public void DadoQueExisteUmPagamentoAprovadoComOOrderId(int orderId)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                OrderNumber = "12345",
                PaymentStatus = (int)PaymentStatus.Approved,
                PaymentDateProcessed = DateTime.UtcNow
            };

            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(orderId))
                .ReturnsAsync(payment);
        }

        [Given(@"que não existe nenhum pagamento com o OrderId (.*)")]
        public void DadoQueNãoExisteNenhumPagamentoComOOrderId(int orderId)
        {
            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(orderId))
                .ReturnsAsync((Payment?)null);
        }

        [When(@"eu consultar o status de pagamento para o pedido com OrderId (.*)")]
        public async Task QuandoEuConsultarOStatusDePagamentoParaOPedidoComOrderId(int orderId)
        {
            _result = await _getPaymentStatus.ExecuteAsync(orderId);
        }

        [Then(@"o status do pagamento deve ser (.*)")]
        public void EntãoOStatusDoPagamentoDeveSer(string expectedStatus)
        {
            Assert.NotNull(_result);
            Assert.Equal(expectedStatus, _result?.Status);
        }

        [Then(@"o número do pedido deve ser (.*)")]
        public void EntãoONúmeroDoPedidoDeveSer(string expectedOrderNumber)
        {
            Assert.NotNull(_result);
            Assert.Equal(expectedOrderNumber, _result?.OrderNumber);
        }

        [Then(@"a data de processamento do pagamento deve ser uma data válida")]
        public void EntãoADataDeProcessamentoDoPagamentoDeveSerUmaDataVálida()
        {
            Assert.NotNull(_result?.PaymentDateProcessed);
            Assert.True(_result?.PaymentDateProcessed < DateTime.UtcNow);
        }

        [Then(@"o status de pagamento deve ser nulo")]
        public void EntãoOStatusDePagamentoDeveSerNulo()
        {
            Assert.Null(_result);
        }
    }
}
