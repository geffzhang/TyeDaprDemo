using System.Collections.Generic;
using System.Threading.Tasks;
using ContosoCrafts.Web.Server.Services;
using ContosoCrafts.Web.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.Web.Server
{
    [ApiController]
    [Route("api")]
    public class LocalApiController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<LocalApiController> logger;

        public LocalApiController(IProductService productService, ILogger<LocalApiController> logger)
        {
            this.logger = logger;
            this.productService = productService;
        }

        [HttpGet("products")]
        public async Task<ActionResult> GetProducts()
        {
            var products = await productService.GetProducts();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult> GetProduct(string id)
        {
            var product = await productService.GetProduct(id);
            return Ok(product);
        }

        [HttpPut("products/{id}")]
        public async Task<ActionResult> UpdateProduct(string id, dynamic payload)
        {
            await Task.CompletedTask;
            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout(IEnumerable<CartItem> items)
        {
            await productService.CheckOut(items);
            return Ok();
        }
    }
}