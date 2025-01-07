# HybridCacheApi Projesi

Bu proje, **HybridCache** kullanarak veri önbellekleme (caching) işlemlerini gerçekleştiren bir API servisi örneğidir. `ProductService` sınıfı, ürünleri kategoriye göre veritabanından almak, önbellekte tutmak, silmek ve etiketlere göre geçersiz kılmak için çeşitli metodlar içerir.

## HybridCache Çalışma Mekanizması (Net9)

HybridCache, **L1 (Yerel Bellek)** ve **L2 (Dağıtılmış Önbellek)** olmak üzere iki farklı seviyede veri önbellekleme yaparak yüksek performanslı veri erişimini sağlar.

1. **Cache Lookup (Önbellek Araması)**:
   - Yöntem, önce **L1** (MemoryCache) önbelleğinde cacheKey değerini arar.
   - Eğer **L1**'de bulamazsa, **L2** (Redis) önbelleğinde arama yapılır.
   
2. **Cache Miss (Önbellek Bulunmaması)**:
   - Eğer veriler **L1** ve **L2**'de bulunmazsa, `FetchProductsFromDatabaseAsync` fonksiyonu çağrılarak veritabanından veri alınır.

3. **Verinin Önbelleğe Alınması**:
   - Veritabanından alınan veri, her iki önbelleğe (L1 ve L2) belirtilen son kullanma süreleriyle kaydedilir.

4. **Yanıt**:
   - Yöntem, veriyi ya **L1** veya **L2** önbelleğinden ya da veritabanından alır ve geri döndürür.

---

## Avantajlar

### 1. **Performans Optimizasyonu**:
   - Sık erişilen veriler, **L1** önbelleğinden hızlıca sunulur.
   - Dağıtılmış örnekler arasında veri tutarlılığı, **L2** önbelleği ile sağlanır.

### 2. **Otomatik Senkronizasyon**:
   - HybridCache, **L1** ve **L2** önbellekleri arasındaki senkronizasyonu otomatik olarak yönetir ve geliştirici yükünü azaltır.

### 3. **Merkezi Son Kullanma Yönetimi**:
   - **L1** ve **L2** seviyelerinde önbellek sürelerini tek bir yapılandırma ile kontrol edebilirsiniz.

### 4. **Nazik Bozulma (Graceful Degradation)**:
   - **L1** önbelleği süresi dolarsa, **L2** önbelleği verilerin hala mevcut olmasını sağlar ve veritabanına sorgu gönderilmesini engeller.

Bu avantajlar, .NET 9 **HybridCache**'i, **dağıtık API uygulamalarında** ürün listeleri gibi ağır ve serileştirilmiş verilerin önbelleklenmesi için ideal hale getirir.

----

## Özellikler

- **HybridCache**: Bu yapı, hem L1 (yerel bellek) hem de L2 (dağıtılmış önbellek) seviyelerinde verileri önbelleğe alır.
    - **MemoryCache (L1)**: Yerel bellek önbelleği kullanılır.
    - **Redis (L2)**: Dağıtılmış önbellek olarak Redis kullanılır.

### 1. Ürünleri Kategorilere Göre Getir

`GetProductsByCategoryAsync` metodu, ürünleri belirli bir kategoriye göre önce yerel bellek (L1) ve ardından Redis (L2) önbelleğinden alır. Eğer veriler önbellekten alınamazsa, veritabanından alınarak her iki önbelleğe kaydedilir.

### 2. Ürünleri Önbellekten Silme

`RemoveProductsByCategoryFromCacheAsync` metodu, belirli bir kategoriye ait ürünlerin L1 ve L2 önbelleklerinden silinmesini sağlar.

### 3. Ürünleri Önbelleğe Ekleme

`AddProductsToCacheAsync` metodu, ürünleri hem yerel bellek (L1) hem de Redis (L2) önbelleklerine ekler. Ayrıca, **Tags** kullanılarak kategoriye ait ürünler etiketlenebilir ve bu etiketlere göre geçersiz kılma işlemi yapılabilir.

### 4. Etikete Göre Önbelleği Geçersiz Kılma

`InvalidateCacheByTagAsync` metodu, belirli bir etiket üzerinden ilişkilendirilmiş tüm önbellek girişlerini siler.

### 5. Veritabanından Ürün Getirme

`FetchProductsFromDatabaseAsync` metodu, veritabanından ürün verilerini alır. Gerçek uygulamalarda veritabanı bağlantısı kurularak veri çekme işlemi yapılır.



 
 
