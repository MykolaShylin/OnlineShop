﻿using AutoMapper;
using System;

namespace OnlineShopWebApp.FeedbackApi.Models
{

    public class Feedback
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Login { get; set; }
        public string Text { get; set; }

        public int Grade { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
