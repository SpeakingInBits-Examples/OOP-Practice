using Microsoft.EntityFrameworkCore;
using OOP_Practice.Data;
using OOP_Practice.Models;

namespace OOP_Practice.Services;

public interface IProductService
{
    /// <summary>
    /// Asynchronously retrieves all products available in the data store.
    /// </summary>
    /// <returns>Returns a collection of all products. The
    /// collection is empty if no products are found.</returns>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    /// Asynchronously retrieves the product with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to retrieve.</param>
    /// <returns>The task result contains the <see cref="Product"/> if found;
    /// otherwise, <see langword="null"/>.</returns>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Asynchronously adds a new product to the data store.
    /// </summary>
    /// <param name="product">The product to add. Cannot be null. All required properties of the product must be set.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddProductAsync(Product product);

    /// <summary>
    /// Asynchronously updates the specified product in the data store.
    /// </summary>
    /// <param name="product">The product to update. The product's identifier should correspond to an existing product in
    /// the data store.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateProductAsync(Product product);

    /// <summary>
    /// Asynchronously deletes the product with the specified identifier if it exists.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    Task DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Product.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Product.FindAsync(id);
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Product.FindAsync(id);
        if (product != null)
        {
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}