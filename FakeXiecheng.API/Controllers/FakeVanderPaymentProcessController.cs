using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/FakeVanderPaymentProcess")]
    public class FakeVanderPaymentProcessController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(
            [FromQuery] Guid orderNumber,
            [FromQuery] bool returnFault)
        {
            await Task.Delay(3000);

            if (returnFault)
            {
                return Ok(new
                {
                    id = Guid.NewGuid(),
                    created = DateTime.UtcNow,
                    approved = false,
                    message = "Reject",
                    payment_method = "信用卡支付",
                    order_number = orderNumber,
                    card = new
                    {
                        card_type = "信用卡",
                        last_four = "1234"
                    }
                });
            }

            return Ok(new
            {
                id = Guid.NewGuid(),
                created = DateTime.UtcNow,
                approved = true,
                message = "Approve",
                payment_method = "信用卡支付",
                order_number = orderNumber,
                card = new
                {
                    card_type = "信用卡",
                    last_four = "1234"
                }
            });
        }
    }
}