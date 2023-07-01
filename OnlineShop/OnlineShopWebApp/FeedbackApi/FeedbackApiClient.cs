using OnlineShopWebApp.FeedbackApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OnlineShopWebApp.FeedbackApi
{
    public class FeedbackApiClient
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public FeedbackApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<Feedback> GetFeedbacks(int productId) 
        {
            var httpClient = _httpClientFactory.CreateClient("FeedbackApi");
            return null;
        }

        public void Add(AddFeedbackModel newFeedback)
        {
            return;
        }
    }
}
