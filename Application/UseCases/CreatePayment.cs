using Application.DTOs;
using Application.Interfaces.ExternalServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.UseCases;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases
{
    public class CreatePayment : ICreatePayment
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IOrderService _orderService;

        public CreatePayment(IPaymentRepository paymentRepository, IMercadoPagoService mercadoPagoService, IOrderService orderService)
        {
            _paymentRepository = paymentRepository;
            _mercadoPagoService = mercadoPagoService;
            _orderService = orderService;
        }

        public async Task<string?> ExecuteAsync(PaymentRequestDto paymentRequest)
        {
            var order = await _orderService.GetOrderByOrderNumberAsync(paymentRequest.OrderNumber);            

            if (order == null)
                throw new Exception("Order not found.");

            var paymentResult = await _paymentRepository.GetByOrderIdAsync(order.Id);
            if (paymentResult != null)
                return paymentResult.QrData;

            var isOrderCreated = await _mercadoPagoService.GenerateOrderMPAsync(order);
            if(!isOrderCreated)
                throw new Exception("Ocorreu um erro ao Criar Pedido no Mercado Pago.");

            var qrCode = await _mercadoPagoService.GenerateQrCodeMPAsync(order);

            var payment = new Payment
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                PaymentStatus = (int) PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow,
                InStoreOrderId = qrCode.InStoreOrderId,
                QrData = qrCode.QrData
            };

            await _paymentRepository.AddAsync(payment);

            return qrCode.QrData;
        }
    }
}
