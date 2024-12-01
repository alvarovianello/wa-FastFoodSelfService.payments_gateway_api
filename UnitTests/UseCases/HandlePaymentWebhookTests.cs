using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.UseCases;
using Domain.Entities;
using Domain.Enums;
using Moq;
using Xunit;

namespace UnitTests.UseCases
{
    public class HandlePaymentWebhookTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IMercadoPagoService> _mercadoPagoServiceMock;
        private readonly HandlePaymentWebhook _handlePaymentWebhook;

        public HandlePaymentWebhookTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            _handlePaymentWebhook = new HandlePaymentWebhook(
                _paymentRepositoryMock.Object,
                _mercadoPagoServiceMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyString_WhenTypeIsNotPayment()
        {
            // Arrange
            var webhookRequest = new WebhookDto { Type = "not-payment", Data = new WebhookDataDto { Id = "12345" } };

            // Act
            var result = await _handlePaymentWebhook.ExecuteAsync(webhookRequest);

            // Assert
            Assert.Equal("", result);
            _mercadoPagoServiceMock.Verify(service => service.GetPaymentAsync(It.IsAny<string>()), Times.Never);
            _paymentRepositoryMock.Verify(repo => repo.UpdatePaymentStatusAsync(It.IsAny<Payment>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenPaymentStatusIsInvalid()
        {
            // Arrange
            var webhookRequest = new WebhookDto { Type = "payment", Data = new WebhookDataDto { Id = "12345" } };
            var paymentResponse = new PaymentMPResponseDto
            {
                ExternalReference = "ORD12345",
                PaymentTypeId = "credit_card",
                Status = "invalid_status",
                Order = new OrderMPDto { Id = 1 }
            };

            _mercadoPagoServiceMock
                .Setup(service => service.GetPaymentAsync(webhookRequest.Data.Id.ToString()))
                .ReturnsAsync(paymentResponse);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _handlePaymentWebhook.ExecuteAsync(webhookRequest));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdatePaymentStatus_WhenPaymentIsValid()
        {
            // Arrange
            var webhookRequest = new WebhookDto { Type = "payment", Data = new WebhookDataDto { Id = "12345" } };
            var paymentResponse = new PaymentMPResponseDto
            {
                ExternalReference = "ORD12345",
                PaymentTypeId = "credit_card",
                Status = "approved",
                Order = new OrderMPDto { Id = 1 }
            };

            _mercadoPagoServiceMock
                .Setup(service => service.GetPaymentAsync(webhookRequest.Data.Id.ToString()))
                .ReturnsAsync(paymentResponse);

            _paymentRepositoryMock
                .Setup(repo => repo.UpdatePaymentStatusAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handlePaymentWebhook.ExecuteAsync(webhookRequest);

            // Assert
            Assert.Equal("", result);
            _paymentRepositoryMock.Verify(repo => repo.UpdatePaymentStatusAsync(It.Is<Payment>(p =>
                p.OrderNumber == "12345" &&
                p.PaymentMethod == "credit_card" &&
                p.PaymentStatus == (int)PaymentStatus.Approved &&
                p.PaymentDate != null)), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldNotUpdatePaymentStatus_WhenPaymentResponseIsNull()
        {
            // Arrange
            var webhookRequest = new WebhookDto { Type = "payment", Data = new WebhookDataDto { Id = "12345" } };

            _mercadoPagoServiceMock
                .Setup(service => service.GetPaymentAsync(webhookRequest.Data.Id.ToString()))
                .ReturnsAsync((PaymentMPResponseDto)null);

            // Act
            var result = await _handlePaymentWebhook.ExecuteAsync(webhookRequest);

            // Assert
            Assert.Equal("", result);
            _paymentRepositoryMock.Verify(repo => repo.UpdatePaymentStatusAsync(It.IsAny<Payment>()), Times.Never);
        }
    }
}
