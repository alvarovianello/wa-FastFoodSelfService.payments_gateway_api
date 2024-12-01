using Application.DTOs;
using Application.Interfaces.UseCases;
using CrossCutting.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace IntegrationTests.Controllers
{
    public class PaymentControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<ICreatePayment> _createPaymentMock = new();
        private readonly Mock<IGetPaymentStatus> _getPaymentStatusMock = new();
        private readonly Mock<IHandlePaymentWebhook> _handlePaymentWebhookMock = new();
        private readonly Mock<IGetApprovedPayments> _getApprovedPaymentsMock = new();
        private readonly Mock<HmacVerifierHelper> _hmacVerifierMock = new();

        public PaymentControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_createPaymentMock.Object);
                    services.AddSingleton(_getPaymentStatusMock.Object);
                    services.AddSingleton(_handlePaymentWebhookMock.Object);
                    services.AddSingleton(_getApprovedPaymentsMock.Object);
                    services.AddSingleton(_hmacVerifierMock.Object);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task PaymentCheckout_ShouldReturnQRCode_WhenSuccessful()
        {
            // Arrange
            var paymentRequest = new PaymentRequestDto
            {
                OrderNumber = "ORD12345"
            };
            var qrCode = "https://example.com/qr-code";
            _createPaymentMock.Setup(x => x.ExecuteAsync(It.IsAny<PaymentRequestDto>())).ReturnsAsync(qrCode);

            // Act
            var response = await _client.PostAsJsonAsync("/api/Payment/payment/checkout", paymentRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.NotNull(responseContent);

            var actualQrCode = responseContent.GetProperty("qrCode").GetString();
            Assert.Equal(qrCode, actualQrCode);
        }



        [Fact]
        public async Task GetPaymentStatus_ShouldReturnPaymentStatus_WhenValidOrderId()
        {
            // Arrange
            var orderId = 1;
            var paymentStatus = new PaymentStatusDto
            {
                OrderId = orderId,
                Status = "Approved",
                PaymentDateProcessed = DateTime.UtcNow
            };
            _getPaymentStatusMock.Setup(x => x.ExecuteAsync(orderId)).ReturnsAsync(paymentStatus);

            // Act
            var response = await _client.GetAsync($"/api/Payment/order/{orderId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadFromJsonAsync<PaymentStatusDto>();
            Assert.Equal(paymentStatus.OrderId, responseContent.OrderId);
            Assert.Equal(paymentStatus.Status, responseContent.Status);
        }

        [Fact]
        public async Task HandleWebhook_ShouldReturnOk_WhenValidWebhook()
        {
            // Arrange
            var webhookRequest = new WebhookDto
            {
                Type = "payment",
                Data = new WebhookDataDto { Id = "12345" }
            };
            var webhookResponse = "Processed Successfully";
            _handlePaymentWebhookMock.Setup(x => x.ExecuteAsync(It.IsAny<WebhookDto>())).ReturnsAsync(webhookResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/api/Payment/webhook", webhookRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains(webhookResponse, responseContent);
        }

        [Fact]
        public async Task GetApprovedPayments_ShouldReturnListOfPayments_WhenValidLimit()
        {
            // Arrange
            var limit = 5;
            var approvedPayments = new List<PaymentStatusDto>
            {
                new PaymentStatusDto { OrderId = 1, Status = "Approved", PaymentDateProcessed = DateTime.UtcNow },
                new PaymentStatusDto { OrderId = 2, Status = "Approved", PaymentDateProcessed = DateTime.UtcNow }
            };
            _getApprovedPaymentsMock.Setup(x => x.ExecuteAsync(limit)).ReturnsAsync(approvedPayments);

            // Act
            var response = await _client.GetAsync($"/api/Payment/approved/{limit}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadFromJsonAsync<List<PaymentStatusDto>>();
            Assert.Equal(approvedPayments.Count, responseContent.Count);
        }
    }
}
