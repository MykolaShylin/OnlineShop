﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models
{
    public class NewComfirmedOrderEventArgs
    {
        public User User { get; set; }
        public Order Order { get; set; }
    }
}
