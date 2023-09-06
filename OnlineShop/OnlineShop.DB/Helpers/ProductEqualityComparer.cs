using OnlineShop.DB.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Helpers
{
    public class ProductEqualityComparer : IEqualityComparer<Product>
    {
        public bool Equals(Product? x, Product? y)
        {
            if(x == null && y == null) return true;
            if(x == null|| y == null) return false;

            return x.Id == y.Id;
        }

        public int GetHashCode(Product obj)
        {
            return obj.Id.GetHashCode();    
        }
    }
}
