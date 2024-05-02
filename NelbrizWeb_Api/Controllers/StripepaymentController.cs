using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nelbriz_Models;
using Stripe.Checkout;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StripepaymentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StripepaymentController(IConfiguration config)
        {
            _config = config;
        }



        [Authorize]
        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create([FromBody] StripePaymentDTO paymentDTO)
        {


            try
            {
                var domain = _config.GetValue<string>("Nelbriz_Client_URL");

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain+paymentDTO.SuccessUrl,
                    CancelUrl = domain+paymentDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                foreach (var item in paymentDTO.Order.OrderDetails)
                {
                    var sessionItemSession = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//20.00 --> 20000
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            },
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionItemSession);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                return Ok(new SuccessModelDTO()
                {
                    Data = session.Id
                });

            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = ex.Message,
                });
            }

        }








    }



}
