using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_DataAccess.ViewModel;
using Nelbriz_Models;
using System.Diagnostics.Eventing.Reader;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private const string OrderCacheKey = "OrderList";
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderController> _logger;
        private IMemoryCache _cache;
        public OrderController(IOrderRepository orderRepository, IMemoryCache cache, ILogger<OrderController> logger)
        {
            _orderRepository = orderRepository;
            _cache = cache;
            _logger = logger;   
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderRepository.GetAll());
        }


        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int? orderHeaderId)
        {
            if (orderHeaderId == null || orderHeaderId == 0)
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            var orderHeader = await _orderRepository.Get(orderHeaderId.Value);
            if (orderHeader == null)
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            return Ok(orderHeader);
        }


        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> Create([FromBody] StripePaymentDTO paymentDTO)
        {
            paymentDTO.Order.OrderHeader.OrderDate = DateTime.Now;
            var result = await _orderRepository.Create(paymentDTO.Order);
            return Ok(result);
        }


    }
}
