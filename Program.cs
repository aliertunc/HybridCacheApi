using Microsoft.Extensions.Caching.Hybrid;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024; 
            options.MaximumKeyLength = 512;
            options.DefaultEntryOptions = new HybridCacheEntryOptions 
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(30)
            };
        });


        //optional
        //builder.Services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = "connectionString";
        //});



        builder.Services.AddControllers();

        var app = builder.Build();
         
        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}