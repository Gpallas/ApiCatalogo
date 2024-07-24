using ApiCatalogo.Context;
using ApiCatalogo.DTOs;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Filters;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using ApiCatalogo.Repositories;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace ApiCatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public CategoriasController(IUnitOfWork uow, IConfiguration configuration, ILogger<CategoriasController> logger)
        {
            _uow = uow;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("LerArquivoConfiguracao")]
        [Authorize]
        public string GetValores()
        {
            var valor1 = _configuration["chave1"];
            var valor2 = _configuration["chave2"];

            var secao1 = _configuration["secao1:chave2"];

            return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
        }

        //Até o .Net 7, precisava usar o [FromServices]
        [HttpGet("UsandoFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }
        
        //À partir do .Net 8, o [FromServices] é implícito por padrão
        [HttpGet("SemUsarFromServices/{nome}")]
        public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        // Actions basicas
        /*[HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            _logger.LogInformation("======================== Get categorias/produtos ============================");
            //Nunca retornar objetos relacionados sem usar algum filtro (where, nesse caso)
            return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).ToList();
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
        {
            //Usar AsNoTracking pra consultas somente de leitura melhora o desempenho por não guardar em cache
            return await _context.Categorias.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<Categoria>> GetAsync(int id)
        {
            _logger.LogInformation($"======================== Get categorias/produtos/id = {id} ============================");
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogInformation($"======================== Get categorias/produtos/id = {id} NÃO ENCONTRADO ============================");
                return NotFound("Categoria não encontrada");
            }

            return Ok(categoria);
        }*/

        // Actions usando o UnitOfWork
        /*[HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var categorias = _uow.CategoriaRepository.GetAll();
            return Ok(categorias);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            _logger.LogInformation($"======================== Get categorias/produtos/id = {id} ============================");
            var categoria = _uow.CategoriaRepository.GetByPredicate(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogInformation($"======================== Get categorias/produtos/id = {id} NÃO ENCONTRADO ============================");
                return NotFound("Categoria não encontrada");
            }

            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
            {
                return BadRequest();
            }

            var categoriaCriada = _uow.CategoriaRepository.Create(categoria);
            _uow.Commit();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }

            var categoriaAtualizada = _uow.CategoriaRepository.Update(categoria);
            _uow.Commit();

            return Ok(categoriaAtualizada);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _uow.CategoriaRepository.GetByPredicate(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound($"Categoria de id {id} não encontrada");
            }

            var categoriaExcluida = _uow.CategoriaRepository.Delete(categoria);
            _uow.Commit();

            return Ok(categoriaExcluida);
        }*/

        // Actions usando DTO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            var categorias = await _uow.CategoriaRepository.GetAllAsync();

            if (categorias is null)
            {
                return NotFound("Não existem categorias");
            }

            //var categoriasDTO = new List<CategoriaDTO>();
            //foreach (var categoria in categorias)
            //{
            //    var categoriaDTO = new CategoriaDTO
            //    {
            //        CategoriaId = categoria.CategoriaId,
            //        Nome = categoria.Nome,
            //        ImagemUrl = categoria.ImagemUrl
            //    };

            //    categoriasDTO.Add(categoriaDTO);
            //}

            var categoriasDTO = categorias.ToCategoriaDTOList();

            return Ok(categoriasDTO);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParams)
        {
            var categorias = await _uow.CategoriaRepository.GetCategoriasAsync(categoriasParams);
            
            return ObterCategorias(categorias);
        }

        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasFiltroNome categoriasFiltroParams)
        {
            var categorias = await _uow.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltroParams);

            return ObterCategorias(categorias);
        }

        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
        {
            var metadata = new
            {
                categorias.Count,
                categorias.PageSize,
                categorias.PageCount,
                categorias.TotalItemCount,
                categorias.HasNextPage,
                categorias.HasPreviousPage
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDTO = categorias.ToCategoriaDTOList();

            return Ok(categoriasDTO);
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            _logger.LogInformation($"======================== Get categorias/produtos/id = {id} ============================");
            var categoria = await _uow.CategoriaRepository.GetByPredicateAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                _logger.LogInformation($"======================== Get categorias/produtos/id = {id} NÃO ENCONTRADO ============================");
                return NotFound("Categoria não encontrada");
            }

            //var categoriaDTO = new CategoriaDTO()
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImagemUrl = categoria.ImagemUrl
            //};

            var categoriaDTO = categoria.ToCategoriaDTO();

            return Ok(categoriaDTO);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO is null)
            {
                return BadRequest();
            }

            //var categoria = new Categoria()
            //{
            //    CategoriaId = categoriaDTO.CategoriaId,
            //    Nome = categoriaDTO.Nome,
            //    ImagemUrl = categoriaDTO.ImagemUrl
            //};

            var categoria = categoriaDTO.ToCategoria();

            var categoriaCriada = _uow.CategoriaRepository.Create(categoria);
            await _uow.CommitAsync();

            //var novaCategoriaDTO = new CategoriaDTO()
            //{
            //    CategoriaId = categoriaCriada.CategoriaId,
            //    Nome = categoriaCriada.Nome,
            //    ImagemUrl = categoriaCriada.ImagemUrl
            //};

            var novaCategoriaDTO = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDTO)
        {
            if (id != categoriaDTO.CategoriaId)
            {
                return BadRequest("Dados inválidos");
            }

            //var categoria = new Categoria()
            //{
            //    CategoriaId = categoriaDTO.CategoriaId,
            //    Nome = categoriaDTO.Nome,
            //    ImagemUrl = categoriaDTO.ImagemUrl
            //};

            var categoria = categoriaDTO.ToCategoria();

            var categoriaAtualizada = _uow.CategoriaRepository.Update(categoria);
            await _uow.CommitAsync();


            //var categoriaAtualizadaDTO = new CategoriaDTO()
            //{
            //    CategoriaId = categoriaAtualizada.CategoriaId,
            //    Nome = categoriaAtualizada.Nome,
            //    ImagemUrl = categoriaAtualizada.ImagemUrl
            //};

            var categoriaAtualizadaDTO = categoriaAtualizada.ToCategoriaDTO();

            return Ok(categoriaAtualizada);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uow.CategoriaRepository.GetByPredicateAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound($"Categoria de id {id} não encontrada");
            }

            var categoriaExcluida = _uow.CategoriaRepository.Delete(categoria);
            await _uow.CommitAsync();

            //var categoriaExcluidaDTO = new CategoriaDTO()
            //{
            //    CategoriaId = categoriaExcluida.CategoriaId,
            //    Nome = categoriaExcluida.Nome,
            //    ImagemUrl = categoriaExcluida.ImagemUrl
            //};

            var categoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();

            return Ok(categoriaExcluidaDTO);
        }
    }
}
