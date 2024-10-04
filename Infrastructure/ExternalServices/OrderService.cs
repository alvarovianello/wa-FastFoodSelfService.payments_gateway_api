using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Infrastructure.ExternalServices
{
    public class OrderService: IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OrderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<OrderReponseDto?> GetOrderByOrderNumberAsync(string orderNumber)
        {
            var orderApiUrl = _configuration["ExternalServices:OrderApiUrl"];
            var response = await _httpClient.GetAsync($"{orderApiUrl}/api/order/summary/{orderNumber}");

            if (!response.IsSuccessStatusCode)
            {
                // Lida com erro
                return null;
            }

            var order = await response.Content.ReadFromJsonAsync<OrderReponseDto>();
            return order;
        }
    }
}
