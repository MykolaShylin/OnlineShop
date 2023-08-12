using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IGoogleMap
    {
        List<GoogleMapShopInfo> GetAll();
    }
}
