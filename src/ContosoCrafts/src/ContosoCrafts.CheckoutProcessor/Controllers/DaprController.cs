using System.Threading.Tasks;
using CloudNative.CloudEvents;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContosoCrafts.CheckoutProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DaprController : ControllerBase
    {
        private readonly ILogger<DaprController> logger;
        public DaprController(ILogger<DaprController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("/checkout")]
        [Topic("rabbitmqbus", "checkout")]
        public async Task<ActionResult> CheckoutOrder()
        {
            var cloudEvent = await this.Request.ReadCloudEventAsync();
            logger.LogDebug($"Cloud event {cloudEvent.Id} {cloudEvent.Type} {cloudEvent.DataContentType}");
            logger.LogInformation("Order received...");
            return Ok();
        }
    }
}
