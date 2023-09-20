

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.DB.Contexts;
using OnlineShop.DB.Models;
using OnlineShop.DB.Models.Enumerations;
using OnlineShop.DB.Models.Interfaces;

namespace OnlineShop.DB.Storages
{
    public class FlavorsDbStorage : IFlavor
    {
        private readonly DataBaseContext dataBaseContext;
        private readonly IMemoryCache _cache;
        public FlavorsDbStorage(DataBaseContext dataBaseContext, IMemoryCache cache)
        {
            this.dataBaseContext = dataBaseContext;
            _cache = cache;
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
            if (_cache.TryGetValue(id, out Flavor? flavor))
            {
                return flavor;
            }
            flavor = await dataBaseContext.Flavors.FirstOrDefaultAsync(flavor => flavor.Id == id);
            if (flavor != null)
            {
                _cache.Set(id, flavor, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return flavor;
        }
        public async Task<Flavor> TryGetByNameAsync(string name)
        {
            if (_cache.TryGetValue(name, out Flavor? flavor))
            {
                return flavor;
            }
            flavor = await dataBaseContext.Flavors.FirstOrDefaultAsync(flav => flav.Name == name);
            if (flavor != null)
            {
                _cache.Set(name, flavor, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return flavor;
        }
        public async Task<List<Flavor>> GetAllAsync()
        {
            return await dataBaseContext.Flavors.ToListAsync();
        }
    }
}
