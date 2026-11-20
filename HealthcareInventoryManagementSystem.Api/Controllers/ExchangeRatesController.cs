
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ExchangeRatesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("PKR")]
        public async Task<IActionResult> GetPkrExchangeRates(
            CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();

            // Primary exchange-rate API.
            const string primaryUrl =
                "https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/pkr.min.json";

            // Fallback exchange-rate API.
            const string fallbackUrl =
                "https://latest.currency-api.pages.dev/v1/currencies/pkr.min.json";

            try
            {
                using var response = await client.GetAsync(
                    primaryUrl,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                return Content(json, "application/json");
            }
            catch (HttpRequestException)
            {
                try
                {
                    using var response = await client.GetAsync(
                        fallbackUrl,
                        cancellationToken);

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync(
                        cancellationToken);

                    return Content(json, "application/json");
                }
                catch (HttpRequestException)
                {
                    return StatusCode(
                        StatusCodes.Status502BadGateway,
                        new
                        {
                            message = "Unable to retrieve exchange rates from the currency providers."
                        });
                }
            }
        }
    }
}
