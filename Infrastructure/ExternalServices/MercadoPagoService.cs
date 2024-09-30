using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MercadoPagoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private object GeneratePayloadOrder(OrderReponseDto order)
        {

            var notificationUrl = _configuration["ExternalServices:MercadoPago:NotificationUrl"];

            var payload = new
            {
                cash_out = new
                {
                    amount = 0
                },
                description = "Pedido FastFood",
                external_reference = "ORD" + order.OrderNumber,
                items = order.OrderItems.Select(item => new
                {
                    sku_number = "SKU" + item.ProductId,
                    category = "marketplace",
                    title = item.ProductName,
                    description = item.Description,
                    unit_price = item.PriceItem,
                    quantity = item.Quantity,
                    unit_measure = "unit",
                    total_amount = item.TotalPrice
                }),
                notification_url = notificationUrl,
                title = "Pedido FastFood - OrderNumber: " + order.OrderNumber,
                total_amount = order.TotalPrice

            };

            return payload;
        }

        public async Task<QrCodeResponseDto> GenerateQrCodeMPAsync(OrderReponseDto order)
        {
            var apiUrl = _configuration["ExternalServices:MercadoPago:ApiUrl"] + "/instore/orders/qr/seller/collectors/{user_id}/pos/{external_pos_id}/qrs";
            var userId = _configuration["ExternalServices:MercadoPago:UserId"];
            var externalPosId = _configuration["ExternalServices:MercadoPago:ExternalPosId"];
            var token = _configuration["ExternalServices:MercadoPago:Token"];

            apiUrl = apiUrl?.Replace("{user_id}", userId)
                            .Replace("{external_pos_id}", externalPosId);

            var payload = GeneratePayloadOrder(order);

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao gerar QR Code no Mercado Pago.");
            }

            var result = await response.Content.ReadFromJsonAsync<QrCodeResponseDto>();
            return result;
        }

        public async Task<bool> GenerateOrderMPAsync(OrderReponseDto order)
        {
            var apiUrl = _configuration["ExternalServices:MercadoPago:ApiUrl"] + "/instore/qr/seller/collectors/{user_id}/pos/{external_pos_id}/orders";
            var userId = _configuration["ExternalServices:MercadoPago:UserId"];
            var externalPosId = _configuration["ExternalServices:MercadoPago:ExternalPosId"];
            var token = _configuration["ExternalServices:MercadoPago:Token"];

            apiUrl = apiUrl?.Replace("{user_id}", userId)
                            .Replace("{external_pos_id}", externalPosId);

            var payload = GeneratePayloadOrder(order);

            var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao gerar Pedido no Mercado Pago.");
            }

            return true;
        }

        public async Task<PaymentMPResponseDto?> GetPaymentAsync(string paymentId)
        {
            var apiUrl = _configuration["ExternalServices:MercadoPago:ApiUrl"] + "/v1/payments/{payment_id}";
            var token = _configuration["ExternalServices:MercadoPago:Token"];

            apiUrl = apiUrl?.Replace("{payment_id}", paymentId);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao Buscar Pagamento no Mercado Pago.");
            }

            var result = await response.Content.ReadFromJsonAsync<PaymentMPResponseDto>();

            return result;
        }

        public async Task<MerchantOrdersMPResponseDto?> GetMerchantOrderAsync(string merchantOrder)
        {
            var apiUrl = _configuration["ExternalServices:MercadoPago:ApiUrl"] + "/merchant_orders/{merchant_order}";
            var token = _configuration["ExternalServices:MercadoPago:Token"];

            apiUrl = apiUrl?.Replace("{merchant_order}", merchantOrder);

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao Buscar Merchant Orders no Mercado Pago.");
            }

            var result = await response.Content.ReadFromJsonAsync<MerchantOrdersMPResponseDto>();

            return result;
        }
    }
}
