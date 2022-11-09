using ehop.Order.API.Models;
using eshop.MessageBus;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace ehop.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public IActionResult CreateOrder(CreateOrderRequest createOrderRequest)
        {
            Order.API.Models.Order order = new Models.Order
            {
                CustomerId = createOrderRequest.CustomerId,
                OrderDate = DateTime.Now,
                OrderItems = createOrderRequest.OrderItems,
                OrderState = OrderState.Pending
            };
            order.Id = 12;
            //order db'ye "PENDING" olarak kaydedildi.

            var @event = new OrderCreatedEvent
            {
                CustomerId = createOrderRequest.CustomerId,
                OrderId = order.Id,
                CreditCardInfo = "fake cart  number",
                OrderItems = order.OrderItems.Select(o => new OrderItemMessage
                {
                    Amount = o.Amount,
                    Price = o.UnitPrice,
                    ProductId = o.ProductId
                }).ToList()

            };

            _publishEndpoint.Publish(@event);
            return Ok();
        }
    }
}

/*
 * 1. Sipariş Ekleme eventi fırlar. (OrderCreated)
2. Stocks API’si OrderCreated eventini consume eder.
3. Eğer yeterli stok varsa StockReserved event’i fırlar.
4. Eğer yeterli stok yoksa StockNotReserved event’i fırlar

 5.  Payment API’si StockReserved event’ini consume eder.

 6.  Eğer ödeme alabiliyorsa PaymentCompleted event’i fırlar.

1. Eğer ödeme alamıyorsa PaymentFailed event’i fırlar
2. Orders API PaymentCompleted eventini dinler ve işlem kapanır
3. Order API’si StockNotReserved eventini consume eder ve fırlarsa OrderFailed olarak db’de günceller.
4. Stocks API’si PaymentFailed eventini consume eder ve fırlarsa  stock’ları değiştirir.
5. Order API’si PaymentFailed eventini consume eder ve fırlarsa OrderFailed olarak db’de güncellenir.
 */