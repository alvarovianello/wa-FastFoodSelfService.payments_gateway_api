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
    public class CreatePaymentTests
    {
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IMercadoPagoService> _mercadoPagoServiceMock;
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly CreatePayment _createPayment;

        public CreatePaymentTests()
        {
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            _orderServiceMock = new Mock<IOrderService>();

            _createPayment = new CreatePayment(
                _paymentRepositoryMock.Object,
                _mercadoPagoServiceMock.Object,
                _orderServiceMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var paymentRequest = new PaymentRequestDto { OrderNumber = "12345" };
            _orderServiceMock
                .Setup(service => service.GetOrderByOrderNumberAsync(paymentRequest.OrderNumber))
                .ReturnsAsync((OrderReponseDto)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _createPayment.ExecuteAsync(paymentRequest));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnQrData_WhenPaymentAlreadyExists()
        {
            // Arrange
            var paymentRequest = new PaymentRequestDto { OrderNumber = "12345" };
            var existingPayment = new Payment { Id = 1, OrderId = 1, QrData = "ExistingQrData" };
            var order = new OrderReponseDto { Id = 1, OrderNumber = "12345" };

            _orderServiceMock
                .Setup(service => service.GetOrderByOrderNumberAsync(paymentRequest.OrderNumber))
                .ReturnsAsync(order);
            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(order.Id))
                .ReturnsAsync(existingPayment);

            // Act
            var result = await _createPayment.ExecuteAsync(paymentRequest);

            // Assert
            Assert.Equal("ExistingQrData", result);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldThrowException_WhenOrderCreationFailsInMercadoPago()
        {
            // Arrange
            var paymentRequest = new PaymentRequestDto { OrderNumber = "12345" };
            var order = new OrderReponseDto { Id = 1, OrderNumber = "12345" };

            _orderServiceMock
                .Setup(service => service.GetOrderByOrderNumberAsync(paymentRequest.OrderNumber))
                .ReturnsAsync(order);
            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(order.Id))
                .ReturnsAsync((Payment)null);
            _mercadoPagoServiceMock
                .Setup(service => service.GenerateOrderMPAsync(order))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await _createPayment.ExecuteAsync(paymentRequest));
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCreateAndReturnQrCode_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentRequest = new PaymentRequestDto { OrderNumber = "12345" };
            var order = new OrderReponseDto { Id = 1, OrderNumber = "12345" };
            var qrCode = new QrCodeResponseDto { InStoreOrderId = "Store123", QrData = "NewQrData" };

            _orderServiceMock
                .Setup(service => service.GetOrderByOrderNumberAsync(paymentRequest.OrderNumber))
                .ReturnsAsync(order);
            _paymentRepositoryMock
                .Setup(repo => repo.GetByOrderIdAsync(order.Id))
                .ReturnsAsync((Payment)null);
            _mercadoPagoServiceMock
                .Setup(service => service.GenerateOrderMPAsync(order))
                .ReturnsAsync(true);
            _mercadoPagoServiceMock
                .Setup(service => service.GenerateQrCodeMPAsync(order))
                .ReturnsAsync(qrCode);
            _paymentRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _createPayment.ExecuteAsync(paymentRequest);

            // Assert
            Assert.Equal("NewQrData", result);
            _paymentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldCallGeneratePayloadOrder_WhenPaymentCreated()
        {
            // Arrange
            var order = new OrderReponseDto
            {
                OrderNumber = "ORD12345",
                TotalPrice = 20,
                OrderItems = new List<OrderItemDto>
        {
            new OrderItemDto
            {
                ProductId = 1,
                ProductName = "Hamburguer",
                Description = "Delicioso hambúrguer",
                PriceItem = 10,
                Quantity = 2,
                TotalPrice = 20
            }
        }
            };

            var paymentRequest = new PaymentRequestDto { OrderNumber = order.OrderNumber };
            var paymentRepositoryMock = new Mock<IPaymentRepository>();
            var mercadoPagoServiceMock = new Mock<IMercadoPagoService>();
            var orderServiceMock = new Mock<IOrderService>();

            orderServiceMock.Setup(service => service.GetOrderByOrderNumberAsync(It.IsAny<string>()))
                .ReturnsAsync(order);

            // Configure o mock para garantir que o pedido será criado com sucesso
            mercadoPagoServiceMock.Setup(service => service.GenerateOrderMPAsync(It.IsAny<OrderReponseDto>()))
                .ReturnsAsync(true); // Simula sucesso na criação do pedido

            mercadoPagoServiceMock.Setup(service => service.GenerateQrCodeMPAsync(It.IsAny<OrderReponseDto>()))
                .ReturnsAsync(new QrCodeResponseDto
                {
                    QrData = "QR_CODE_DATA",
                    InStoreOrderId = "ORDER_123"
                });

            var createPayment = new CreatePayment(paymentRepositoryMock.Object, mercadoPagoServiceMock.Object, orderServiceMock.Object);

            // Act
            var result = await createPayment.ExecuteAsync(paymentRequest);

            // Assert
            Assert.NotNull(result);
            mercadoPagoServiceMock.Verify(service => service.GenerateOrderMPAsync(It.IsAny<OrderReponseDto>()), Times.Once);
            mercadoPagoServiceMock.Verify(service => service.GenerateQrCodeMPAsync(It.IsAny<OrderReponseDto>()), Times.Once);
        }

    }
}
