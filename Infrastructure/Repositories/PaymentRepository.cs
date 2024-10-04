using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IConfiguration _configuration;

        public PaymentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task AddAsync(Payment payment)
        {
            var query = @"INSERT INTO dbo.Payment (order_id, order_number, payment_status, payment_date, qr_data, in_store_order_id) 
                      VALUES (@OrderId, @OrderNumber, @PaymentStatus, @PaymentDate, @QrData, @InStoreOrderId) RETURNING id";

            using (var connection = CreateConnection())
            {
                await connection.ExecuteScalarAsync<int>(query, payment);
            }
        }

        public async Task<Payment?> GetByOrderIdAsync(int orderId)
        {
            var query = @"SELECT id, order_id as OrderId, order_number as OrderNumber, payment_method as PaymentMethod, payment_status as PaymentStatus, payment_date as PaymentDate, payment_date_processed as PaymentDateProcessed, qr_data as QrData, in_store_order_id as InStoreOrderId FROM dbo.Payment WHERE order_id = @OrderId";
            using (var connection = CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Payment>(query, new { OrderId = orderId });
            }
        }

        public async Task UpdatePaymentStatusAsync(Payment payment)
        {
            var query = @"UPDATE dbo.Payment SET payment_status = @PaymentStatus, payment_date_processed = @ProcessedDate, payment_method = @PaymentMethod WHERE order_number = @OrderNumber";

            using (var connection = CreateConnection())
            {
                await connection.ExecuteAsync(query, new { 
                            PaymentStatus = payment.PaymentStatus,
                            PaymentMethod = payment.PaymentMethod,
                            ProcessedDate = DateTime.UtcNow, 
                            OrderNumber = payment.OrderNumber
                });
            }
        }

        public async Task<IEnumerable<Payment>> GetApprovedPaymentsAsync(int limit)
        {
            var query = @"SELECT id, order_id as OrderId, order_number as OrderNumber, payment_method as PaymentMethod, 
                          payment_status as PaymentStatus, payment_date as PaymentDate, 
                          payment_date_processed as PaymentDateProcessed, 
                          qr_data as QrData, in_store_order_id as InStoreOrderId 
                  FROM dbo.Payment 
                  WHERE payment_status = @PaymentStatus 
                  ORDER BY payment_date DESC 
                  LIMIT @Limit";

            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<Payment>(query, new { PaymentStatus = (int)PaymentStatus.Approved, Limit = limit });
            }
        }
    }
}
