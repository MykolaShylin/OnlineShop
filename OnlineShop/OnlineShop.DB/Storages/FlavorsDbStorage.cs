

using Microsoft.EntityFrameworkCore;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;

namespace OnlineShop.DB.Storages
{
    public class FlavorsDbStorage : IFlavor
    {
        private readonly DataBaseContext dataBaseContext;
        public FlavorsDbStorage(DataBaseContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext;
        }
        public async Task SaveAsync(Flavor flavor)
        {
            dataBaseContext.Flavors.Add(flavor);
        }
        public async Task DeleteAsync(Flavor flavor)
        {
            dataBaseContext.Flavors.Remove(flavor);
        }

        public async Task<Flavor> TryGetByIdAsync(int id)
        {
            return await dataBaseContext.Flavors.FirstOrDefaultAsync(flavor => flavor.Id == id);
        }
        public async Task<Flavor> TryGetByNameAsync(string name)
        {
            return await dataBaseContext.Flavors.FirstOrDefaultAsync(prod => prod.Name == name);
        }
        public async Task<List<Flavor>> GetAllAsync()
        {
            return await dataBaseContext.Flavors.ToListAsync();
        }
    }
}
