using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_Models;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;  
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _orderRepository.GetAll());
        }


        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int? orderHeaderId)
        {
            if(orderHeaderId == null || orderHeaderId == 0)
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status400BadRequest           
                });
            }
            var orderHeader = await _orderRepository.Get(orderHeaderId.Value);
            if(orderHeader == null) 
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            return Ok(orderHeader);
        }


    }
}
