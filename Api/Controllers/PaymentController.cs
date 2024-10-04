using Application.DTOs;
using Application.Interfaces.UseCases;
using CrossCutting.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.QrCode;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ICreatePayment _createPayment;
        private readonly IGetPaymentStatus _getPaymentStatus;
        private readonly IHandlePaymentWebhook _handlePaymentWebhook;
        private readonly IGetApprovedPayments _getApprovedPayments;
        private readonly HmacVerifierHelper _hmacVerifier;
        private readonly IConfiguration _configuration;

        public PaymentController(
            ICreatePayment createPayment,
            IGetPaymentStatus getPaymentStatus,
            IHandlePaymentWebhook handlePaymentWebhook,
            IGetApprovedPayments getApprovedPayments,
            HmacVerifierHelper hmacVerifier,
            IConfiguration configuration)
        {
            _createPayment = createPayment;
            _getPaymentStatus = getPaymentStatus;
            _handlePaymentWebhook = handlePaymentWebhook;
            _getApprovedPayments = getApprovedPayments;
            _hmacVerifier = hmacVerifier;
            _configuration = configuration;
        }

        [HttpPost("payment/checkout")]
        public async Task<IActionResult> PaymentCheckout([FromBody] PaymentRequestDto paymentRequest)
        {
            try
            {
                var qrCode = await _createPayment.ExecuteAsync(paymentRequest);
                return Ok(new { qrCode = qrCode });
                //var qrCodeImage = GenerateQRCodeImage(qrCode);
                //return File(qrCodeImage, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            try
            {
                var paymentStatus = await _getPaymentStatus.ExecuteAsync(orderId);
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("approved/{limit}")]
        public async Task<IActionResult> GetApprovedPayments(int limit)
        {
            try
            {
                var paymentStatus = await _getApprovedPayments.ExecuteAsync(limit);
                return Ok(paymentStatus);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] WebhookDto request)
        {
            try
            {

                string? xSignature = Request.Headers["X-Signature"];
                string? dataId = request.Data?.Id;
                string? secret = _configuration["ExternalServices:MercadoPago:SecretSignature"];
                string? xRequestId = Request.Headers["X-Request-Id"];

                Console.WriteLine($"Header X-Signature: {xSignature}");
                Console.WriteLine($"Header X-Request-Id: {xRequestId}");
                Console.WriteLine($"Secret: {secret}");
                Console.WriteLine($"Body: {JsonConvert.SerializeObject(request)}");

                if (!string.IsNullOrEmpty(xSignature) && !string.IsNullOrEmpty(dataId) && !string.IsNullOrEmpty(xRequestId)) 
                {
                    var isValid = _hmacVerifier.VerifyHmac(xSignature, dataId, xRequestId, secret);
                }
                    
                //return BadRequest("Header: X-Signature, X-Request-Id are required | Data.Id is required");



                //if (!isValid)
                //{
                //    return BadRequest("HMAC verification failed");
                //}
                //return Ok();

                var msg = await _handlePaymentWebhook.ExecuteAsync(request);
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }

        private byte[] GenerateQRCodeImage(string text)
        {
            var qrCodeWriter = new QRCodeWriter();
            var qrCodeData = qrCodeWriter.encode(text, BarcodeFormat.QR_CODE, 300, 300);

            using (var bitmap = new Bitmap(qrCodeData.Width, qrCodeData.Height))
            {
                for (var y = 0; y < qrCodeData.Height; y++)
                {
                    for (var x = 0; x < qrCodeData.Width; x++)
                    {
                        bitmap.SetPixel(x, y, qrCodeData[x, y] ? Color.Black : Color.White);
                    }
                }

                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }
    }
}
