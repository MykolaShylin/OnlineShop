using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models
{
    public class GoogleMapShopInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string WorkingHours { get; set; }
        public string Phone { get; set; }
        public double GeoLat { get; set; }
        public double GeoLong { get; set; }
    }
}
