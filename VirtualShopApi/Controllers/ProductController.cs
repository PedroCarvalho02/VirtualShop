using LojaVirtualAPI.Data;
using LojaVirtualAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LojaVirtualAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _contexto;

        public ProductController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult ObterProdutos()
        {
            var produtos = _contexto.Products?.ToList();
            if (produtos == null)
                return NotFound();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult ObterProduto(int id)
        {
            var produto = _contexto.Products?.Find(id);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdicionarProduto(Product produto)
        {
            if (_contexto.Products == null)
            {
                return Problem("O conjunto de entidades 'ApplicationDbContext.Products' é nulo.");
            }
            _contexto.Products.Add(produto);
            await _contexto.SaveChangesAsync();
            return Ok(produto);
        }

        [HttpGet("search")]
        public IActionResult PesquisarProdutoPorNome(string nome)
        {
            var produtos = _contexto.Products?.Where(p => p.Name.Contains(nome)).ToList();
            if (produtos == null || produtos.Count == 0)
                return NotFound();

            return Ok(produtos);
        }
    }
}