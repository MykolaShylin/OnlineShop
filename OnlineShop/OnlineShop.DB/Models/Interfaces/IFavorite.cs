using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IFavorite
    {
        Task AddAsync(Product product, string userId);
        Task DeleteAsync(Product product, string userId);

        Task<FavoriteProduct> GetByUserIdAsync(string userId);
    }
}
