using VirtualShopMinimalAPI.Data;
using VirtualShopMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace VirtualShopMinimalAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IResult> AddProduct(Product product)
        {
            if (_db.Products == null)
            {
                return Results.NotFound("Produtos não encontrados.");
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return Results.Created($"/api/Product/{product.Id}", product);
        }

        public async Task<IResult> GetProducts()
        {
            if (_db.Products == null)
            {
                return Results.NotFound("Products collection is null.");
            }

            var products = await _db.Products.ToListAsync();
            return Results.Ok(products);
        }

        public async Task<IResult> SearchProducts(string? name)
        {
            if (_db.Products == null)
            {
                return Results.NotFound("Products collection is null.");
            }

            if (string.IsNullOrEmpty(name))
            {
                return Results.BadRequest("O parâmetro 'nome' é obrigatório.");
            }

            var result = await _db.Products
                .Where(p => p.Nome != null && p.Nome.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return Results.Ok(result);
        }

        public async Task<IResult> DeleteProduct(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return Results.NotFound("Produto não encontrado.");
            }

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Results.Ok(new { Mensagem = "Produto deletado com sucesso" });
        }

        public async Task<IResult> UpdateProduct(int id, Product updatedProduct)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return Results.NotFound("Produto não encontrado.");
            }

            product.Nome = updatedProduct.Nome ?? product.Nome;
            product.Preco = updatedProduct.Preco != 0 ? updatedProduct.Preco : product.Preco;
            product.ImageUrl = updatedProduct.ImageUrl ?? product.ImageUrl;

            await _db.SaveChangesAsync();
            return Results.Ok(product);
        }
    }
}