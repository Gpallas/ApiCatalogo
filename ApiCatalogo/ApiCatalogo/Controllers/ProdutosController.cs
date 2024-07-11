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
        private readonly ISpecificProdutoRepository _repository;
        public ProdutosController(ISpecificProdutoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("primeiro")]
        [HttpGet("/primeiro")]
        public ActionResult<Produto> GetPrimeiro()
        {
            var produto = _repository.GetProdutos().First();

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
            var produtos = _repository.GetProdutos().Take(10).ToList();

            if (produtos is null)
            {
                return NotFound("Lista de produtos não encontrada");
            }

            return produtos;
        }

        [HttpGet("{id:int}/{nome}")]
        public ActionResult<Produto> Get(int id, string nome)
        {
            var produto = _repository.GetProdutos().FirstOrDefault(p => p.ProdutoId == id && p.Nome == nome);

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
            var produto = _repository.GetProdutoById(id);

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

            var novoProduto = _repository.CreateProduto(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return BadRequest();
            }

            bool atualizado =_repository.UpdateProduto(produto);
            
            if (atualizado)
            {
                return Ok(produto);
            }
            else
            {
                return StatusCode(500, $"Falha ao atualizar o produto de id {id}");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            bool deletado = _repository.DeleteProduto(id);

            if (deletado)
            {
                return Ok($"Produto de id {id} foi excluído");
            }
            else
            {
                return StatusCode(500, $"Falha ao excluir o produto de id {id}");
            }
        }
    }
}
