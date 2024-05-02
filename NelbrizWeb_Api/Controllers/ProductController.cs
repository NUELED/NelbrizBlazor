using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Nelbriz_Business.Repository;
using Nelbriz_Business.Repository.IRepository;
using Nelbriz_Common;
using Nelbriz_DataAccess;
using Nelbriz_DataAccess.ViewModel;
using Nelbriz_Models;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private const string ProductCacheKey = "ProductList";
        private readonly ILogger<ProductController> _logger;
        private IMemoryCache _cache;
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);//This helps determine the no. of threads that can access a resource concurrently.



        public ProductController( IProductRepository productRepository, ILogger<ProductController> logger, IMemoryCache cache)
        {
            _productRepository = productRepository; 
            _logger = logger;   
            _cache = cache; 
        }




        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _productRepository.GetAll());

        }


        /// <summary>
        ///  Implementing in-memory  data caching in the application. I used the endpoint below for that.
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettAll_cache")]
        public async Task<IActionResult> GetAll2()
        {
            if (_cache.TryGetValue(ProductCacheKey, out IEnumerable<ProductDTO>? products))
            {
                _logger.LogInformation("Products found in cache.");
            }
            else
            {

                try
                {
                    await semaphoreSlim.WaitAsync();

                    if (_cache.TryGetValue(ProductCacheKey, out products))
                    {
                        _logger.LogInformation("Products found in cache.");
                    }
                    else
                    {

                        _logger.LogInformation("Products  not found in cache. Fetching from  database.");

                        products = await _productRepository.GetAll();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60)) //This would'nt expire, as long as you keep accessimg it.
                            .SetAbsoluteExpiration(TimeSpan.FromHours(1))   // This expires immediately the expiration time is reached,wether you access it or not.
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1);

                        _cache.Set(ProductCacheKey, products, cacheEntryOptions);
                        //_cache.Set("TestKey", products, cacheEntryOptions);
                    }
                }
                finally
                {
                  semaphoreSlim?.Release();
                }


            }
            //return Ok(await _productRepository.GetAll());
            return Ok(products);
        }



        [HttpGet("{productId}")]
        public async Task<IActionResult> Get(int? productId)
        {
            if(productId == null || productId ==0)
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status400BadRequest           
                });
            }
            var product = await _productRepository.Get(productId.Value);
            // _cache.Remove(ProductCacheKey);  // tzhis can be used to remove the cache!!
            if(product == null) 
            {
                return BadRequest(new ErrorModelDTO
                {
                    ErrorMessage = "Invalid Id",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            return Ok(product);
        }




    }
}
