using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.DB.Models.Interfaces
{
    public interface IPictures
    {
        Task SaveAsync(ProductPicture picture);
        Task DeleteAsync(ProductPicture picture);
        Task<ProductPicture> TryGetByPathAsync(string path);
        Task<List<ProductPicture>> GetAllAsync();
    }
}
