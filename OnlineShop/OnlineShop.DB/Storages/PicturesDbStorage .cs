using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;

namespace OnlineShop.DB.Storages
{
    public class PicturesDbStorage : IPictures
    {
        private readonly DataBaseContext dataBaseContext;
        public PicturesDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }
        public async Task SaveAsync(ProductPicture picture)
        {
            dataBaseContext.Pictures.Add(picture);
        }
        public async Task DeleteAsync(ProductPicture picture)
        {
            dataBaseContext.Pictures.RemoveRange(picture);
        }
        
        public async Task<ProductPicture> TryGetByPathAsync(string path)
        {
            return await dataBaseContext.Pictures.FirstOrDefaultAsync(picture => picture.Path == path);
        }
        public async Task<List<ProductPicture>> GetAllAsync()
        {
            return await dataBaseContext.Pictures.ToListAsync();
        }
    }
}
