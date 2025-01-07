using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Caching.Hybrid;

namespace HybridCacheApi
{
    public class ProductService
    {
        private readonly HybridCache _cache;

        #region Net9

      
        public ProductService(HybridCache cache)
        {
            _cache = cache;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"products:category:{category}";

            // HybridCache kullanarak veriyi L1 veya L2'den alın
            return await _cache.GetOrCreateAsync(
                cacheKey,
                async token => await FetchProductsFromDatabaseAsync(category, token),
                new HybridCacheEntryOptions // bu ayar program.cs olarak kıllanılmak istenirse null geçilebilebilir.
                {
                    Expiration = TimeSpan.FromMinutes(30), // L1 ve L2 için ortak son kullanma süresi
                    LocalCacheExpiration = TimeSpan.FromMinutes(5) // L1 son kullanma süresi
                },
                null,
                cancellationToken
            );
        }

        private async Task<List<Product>> FetchProductsFromDatabaseAsync(string category, CancellationToken token)
        {
            var response = new List<Product>();
            return response;

            // Veritabanından ürünleri getirme işlemi
            // ...
        }

        public async Task RemoveProductsByCategoryFromCacheAsync(string category, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"products:category:{category}";

            // Remove the cache entry from both L1 and L2
            await _cache.RemoveAsync(cacheKey, cancellationToken);
        }

        public async Task AddProductsToCacheAsync(List<Product> products, string category, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"products:category:{category}";

            // Önbelleğe ekleme işlemi
            await _cache.SetAsync(
                cacheKey,
                products,
                null,
                new List<string> { $"category:{category}" }, // Tags ekleniyor
                cancellationToken
            );
        }


        public async Task InvalidateCacheByTagAsync(string categoryTag, CancellationToken cancellationToken = default)
        {
            // Use the tag to remove all associated cache entries
            await _cache.RemoveByTagAsync(categoryTag, cancellationToken);
        }

        #endregion

        #region Net8
        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            string cacheKey = $"products:category:{category}";

            // L1 Cache Check
            if (_memoryCache.TryGetValue(cacheKey, out List<Product> products))
            {
                return products;
            }

            // L2 Cache Check
            var cachedData = await _redisCache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                products = JsonSerializer.Deserialize<List<Product>>(cachedData);
                // Populate L1 Cache
                _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
                return products;
            }

            // If not found in caches, fetch from database
            products = await FetchProductsFromDatabaseAsync(category);

            // Cache in both L1 and L2
            _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(5));
            await _redisCache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(products),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) }
            );

            return products;
        }
        #endregion
    }

}
