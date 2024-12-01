using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace UnitTests.UseCases
{
    public class GetPaymentStatusTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly GetPaymentStatus _getPaymentStatus;

        public GetPaymentStatusTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _getPaymentStatus = new GetPaymentStatus(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnNull_WhenPaymentNotFound()
        {
            // Arrange
            var orderId = 1;
            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(orderId))
                .ReturnsAsync((Payment)null);

            // Act
            var result = await _getPaymentStatus.ExecuteAsync(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnPaymentStatusDto_WhenPaymentExists()
        {
            // Arrange
            var orderId = 1;
            var payment = new Payment
            {
                OrderId = orderId,
                PaymentStatus = (int)PaymentStatus.Approved,
                PaymentDateProcessed = DateTime.UtcNow
            };

            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(orderId))
                .ReturnsAsync(payment);

            // Act
            var result = await _getPaymentStatus.ExecuteAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PaymentStatusDto>(result);
            Assert.Equal(orderId, result?.OrderId);
            Assert.Equal(PaymentStatus.Approved.ToString(), result?.Status);
            Assert.Equal(payment.PaymentDateProcessed, result?.PaymentDateProcessed);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectOrderId()
        {
            // Arrange
            var orderId = 5;

            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(orderId))
                .ReturnsAsync((Payment)null);

            // Act
            await _getPaymentStatus.ExecuteAsync(orderId);

            // Assert
            _paymentRepositoryMock.Verify(repo => repo.GetByOrderIdAsync(orderId), Times.Once);
        }
    }
}
