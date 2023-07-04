using OnlineShopWebApp.FeedbackApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OnlineShopWebApp.FeedbackApi
{
    public class FeedbackApiClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        public FeedbackApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient("FeedbackApi");
        }

        public async Task<List<Feedback>> GetFeedbacksAsync(int productId) 
        {
            var feedbacks = await _httpClient.GetFromJsonAsync<List<Feedback>>($"/Feedback/GetAllByProductId?productId={productId}");
            return feedbacks;
        }

        public async Task AddAsync(AddFeedbackModel newFeedback)
        {
            await _httpClient.PostAsJsonAsync("/Feedback/AddFeedback", newFeedback);
        }
    }
}
