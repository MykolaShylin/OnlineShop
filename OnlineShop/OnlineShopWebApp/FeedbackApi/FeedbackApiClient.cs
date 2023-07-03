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

        public FeedbackApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Feedback>> GetFeedbacksAsync(int productId) 
        {
            var httpClient = _httpClientFactory.CreateClient("FeedbackApi");
            var feedbacks = await httpClient.GetFromJsonAsync<List<Feedback>>($"/Feedback/GetAllByProductId?productId={productId}");
            return feedbacks;
        }

        public void Add(AddFeedbackModel newFeedback)
        {
            return;
        }
    }
}
