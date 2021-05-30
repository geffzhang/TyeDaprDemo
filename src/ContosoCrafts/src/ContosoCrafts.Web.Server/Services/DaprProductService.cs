using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ContosoCrafts.Web.Shared.Models;
using Dapr.Client;

namespace ContosoCrafts.Web.Server.Services
{
    public class DaprProductService : IProductService
    {
        private readonly DaprClient daprClient;

        public DaprProductService(DaprClient daprClient)
        {
            this.daprClient = daprClient;
        }

        public async Task AddRating(string productId, int rating)
        {
            var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Put, "productsapi", "products", new { productId, rating });

            await daprClient.InvokeMethodAsync(req);
        }

        public async Task CheckOut(IEnumerable<CartItem> Items)
        {
            await daprClient.PublishEventAsync(Constants.PUBSUB_NAME, "checkout", Items);
        }

        public async Task<Product> GetProduct(string id)
        {
            var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "productsapi", $"products/{id}");
            var resp = await daprClient.InvokeMethodWithResponseAsync(req);

            if (!resp.IsSuccessStatusCode)
            {
                // probably log some stuff here
                return null;
            }

            var product = await resp.Content.ReadFromJsonAsync<Product>();
            return product;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var req = daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "productsapi", "products");
            var resp = await daprClient.InvokeMethodWithResponseAsync(req);

            if (!resp.IsSuccessStatusCode)
            {
                // probably log some stuff here
                return Enumerable.Empty<Product>();
            }

            var products = await resp.Content.ReadFromJsonAsync<IEnumerable<Product>>();
            return products;
        }
    }
}