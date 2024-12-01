using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.UseCases;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace UnitTests.UseCases
{
    public class GetApprovedPaymentsTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly GetApprovedPayments _getApprovedPayments;

        public GetApprovedPaymentsTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _getApprovedPayments = new GetApprovedPayments(_paymentRepositoryMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoApprovedPaymentsExist()
        {
            // Arrange
            _paymentRepositoryMock
                .Setup(repo => repo.GetApprovedPaymentsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Payment>());

            // Act
            var result = await _getApprovedPayments.ExecuteAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnMappedList_WhenApprovedPaymentsExist()
        {
            // Arrange
            var approvedPayments = new List<Payment>
            {
                new Payment
                {
                    OrderId = 1,
                    OrderNumber = "12345",
                    PaymentStatus = (int)PaymentStatus.Approved,
                    PaymentDateProcessed = DateTime.UtcNow
                },
                new Payment
                {
                    OrderId = 2,
                    OrderNumber = "67890",
                    PaymentStatus = (int)PaymentStatus.Approved,
                    PaymentDateProcessed = DateTime.UtcNow.AddMinutes(-30)
                }
            };

            _paymentRepositoryMock
                .Setup(repo => repo.GetApprovedPaymentsAsync(It.IsAny<int>()))
                .ReturnsAsync(approvedPayments);

            // Act
            var result = (await _getApprovedPayments.ExecuteAsync(10)).ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal("12345", result[0].OrderNumber);
            Assert.Equal(PaymentStatus.Approved.ToString(), result[0].Status);

            Assert.Equal("67890", result[1].OrderNumber);
            Assert.Equal(PaymentStatus.Approved.ToString(), result[1].Status);

            Assert.IsType<PaymentStatusDto>(result[0]);
            Assert.IsType<PaymentStatusDto>(result[1]);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallRepositoryWithCorrectLimit()
        {
            // Arrange
            var limit = 5;

            _paymentRepositoryMock
                .Setup(repo => repo.GetApprovedPaymentsAsync(limit))
                .ReturnsAsync(new List<Payment>());

            // Act
            await _getApprovedPayments.ExecuteAsync(limit);

            // Assert
            _paymentRepositoryMock.Verify(repo => repo.GetApprovedPaymentsAsync(limit), Times.Once);
        }
    }
}
