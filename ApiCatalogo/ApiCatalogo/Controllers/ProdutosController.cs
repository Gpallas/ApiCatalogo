using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IRepository<Produto> _repository;
        public ProdutosController(IRepository<Produto> repository, IProdutoRepository produtoRepository)
        {
            _repository = repository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet("produtos/{categoriaId:int}")]
        public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int categoriaId)
        {
            var produtos = _produtoRepository.GetProdutosPorCategoria(categoriaId);

            if (produtos is null)
            {
                return NotFound();
            }

            return Ok(produtos);
        }

        [HttpGet("primeiro")]
        [HttpGet("/primeiro")]
        public ActionResult<Produto> GetPrimeiro()
        {
            var produto = _repository.GetAll().First();

            if (produto is null)
            {
                return NotFound("Não há produtos");
            }

            return Ok(produto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            //Nunca retornar todos os registros numa consulta (take(10), nesse caso)
            var produtos = _repository.GetAll().Take(10).ToList();

            if (produtos is null)
            {
                return NotFound("Lista de produtos não encontrada");
            }

            return produtos;
        }

        [HttpGet("{id:int}/{nome}")]
        public ActionResult<Produto> Get(int id, string nome)
        {
            var produto = _repository.GetByPredicate(p => p.ProdutoId == id && p.Nome == nome);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            return Ok(produto);
        }

        //Não usar a restrição de rotas como validação pra ação. Usar pra distinguir entre rotas similares
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _repository.GetByPredicate(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            return produto;
        }

        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                return BadRequest();
            }

            var novoProduto = _repository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            var produtoAtualizado = _repository.Update(produto);

            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _repository.GetByPredicate(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            var produtoDeletado = _repository.Delete(produto);
            return Ok(produtoDeletado);
        }
    }
}
