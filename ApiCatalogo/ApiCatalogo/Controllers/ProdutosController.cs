using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ApiCatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet("produtos/{categoriaId:int}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int categoriaId)
        {
            var produtos = _uow.ProdutoRepository.GetProdutosPorCategoria(categoriaId);

            if (produtos is null)
            {
                return NotFound();
            }

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet("primeiro")]
        [HttpGet("/primeiro")]
        public ActionResult<ProdutoDTO> GetPrimeiro()
        {
            var produto = _uow.ProdutoRepository.GetAll().First();

            if (produto is null)
            {
                return NotFound("Não há produtos");
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        [HttpGet("pagination")]
        public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] ProdutosParameters produtosParams)
        {
            var produtos = _uow.ProdutoRepository.GetProdutos(produtosParams);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.hasNext,
                produtos.hasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            //Nunca retornar todos os registros numa consulta (take(10), nesse caso)
            var produtos = _uow.ProdutoRepository.GetAll().Take(10).ToList();

            if (produtos is null)
            {
                return NotFound("Lista de produtos não encontrada");
            }

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet("{id:int}/{nome}")]
        public ActionResult<ProdutoDTO> Get(int id, string nome)
        {
            var produto = _uow.ProdutoRepository.GetByPredicate(p => p.ProdutoId == id && p.Nome == nome);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        //Não usar a restrição de rotas como validação pra ação. Usar pra distinguir entre rotas similares
        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _uow.ProdutoRepository.GetByPredicate(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDTO)
        {
            if (produtoDTO is null)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            var novoProduto = _uow.ProdutoRepository.Create(produto);
            _uow.Commit();

            var novoProdutoDTO = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDTO.ProdutoId }, novoProdutoDTO);
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateResponse> Patch(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
            {
                return BadRequest();
            }

            var produto = _uow.ProdutoRepository.GetByPredicate(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound();
            }

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(produtoUpdateRequest, produto);

            _uow.ProdutoRepository.Update(produto);
            _uow.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            var produtoAtualizado = _uow.ProdutoRepository.Update(produto);
            _uow.Commit();

            var produtoAtualizadoDTO = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(produtoAtualizadoDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _uow.ProdutoRepository.GetByPredicate(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado");
            }

            var produtoDeletado = _uow.ProdutoRepository.Delete(produto);
            _uow.Commit();

            var produtoDeletadoDTO = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDTO);
        }
    }
}
