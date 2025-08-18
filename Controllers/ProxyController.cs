using AllPaintsEcomAPI.Helpers;
using AllPaintsEcomAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
namespace AllPaintsEcomAPI.Controllers
{
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;      
          public ProxyController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseUrl = config["Proxy:SheenlacApiBaseUrl"];
        }




        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] object users1)
        {
            try
            {                     
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}Auth/Login", users1);      
                var encryptedResponse = await response.Content.ReadAsStringAsync();                     
                return StatusCode(200,encryptedResponse);
               
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error calling external API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }



    }
}
