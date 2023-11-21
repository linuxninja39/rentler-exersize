using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Rentler.Interview.Lib.Models;

namespace Rentler.Interview.Lib
{
    public class FoodsClient
    {
        private readonly Uri _hostUri;
        private readonly HttpClient _client;
        private const string BasePath = "Foods";
        private readonly string _baseUri;

        public FoodsClient(Uri hostUri)
        {
            _hostUri = hostUri;
            _baseUri = $"{_hostUri}{BasePath}";

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true;

            _client = new HttpClient(handler);
        }

        public async Task<PagedResponseDto<FoodsPreview>> GetAllFoods(int page = 0, int size = 10)
        {
            var uri = $"{_baseUri}?page={page}&size={size}";
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<PagedResponseDto<FoodsPreview>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<PagedResponseDto<Foods>> SearchFoods(
            int page = 0,
            int size = 10,
            string searchTerm = ""
        )
        {
            var uri = $"{_baseUri}?page={page}&size={size}&searchTerm={searchTerm}";
            Console.WriteLine($"uri: {uri}");
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            var pagedResponse = JsonSerializer.Deserialize<PagedResponseDto<Foods>>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return pagedResponse;
        }

        public async Task<Foods> AddFood(Foods food)
        {
            var postContent = JsonSerializer.Serialize(food, new JsonSerializerOptions { IgnoreNullValues = true });
            Console.WriteLine($"food json: {postContent}");
            Console.WriteLine($"baseUrl: {_baseUri}");
            var response = await _client.PostAsync(
                _baseUri,
                new StringContent(postContent, Encoding.UTF8, "application/json")
            );
            response.EnsureSuccessStatusCode();


            var foodJsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"response string{foodJsonString}");
            var foodObj = JsonSerializer.Deserialize<Foods>(
                foodJsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );


            return foodObj;
        }

        public async Task<PagedResponseDto<FoodsPreview>> UpdateFood(Foods food)
        {
            var postContent = JsonSerializer.Serialize(food);
            var response = await _client.PutAsync(
                _baseUri,
                new StringContent(postContent, Encoding.UTF8, "application/json")
            );
            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<PagedResponseDto<FoodsPreview>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task DeleteFood(int id)
        {
            var uri = $"{_baseUri}/{id}";
            var response = await _client.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }
    }
}