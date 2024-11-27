using System;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if(!context.Products.Any()){
            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

            var product = JsonSerializer.Deserialize<List<Product>>(productsData);

            if(product == null)
                return;

            context.Products.AddRange(product);

            await context.SaveChangesAsync();    
        }
    }
}
