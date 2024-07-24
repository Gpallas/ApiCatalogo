using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using X.PagedList;

namespace ApiCatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IPagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriaParams)
        {
            var categorias = await GetAllAsync();
            var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();
            //var resultado = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, categoriaParams.PageNumber, categoriaParams.PageSize);
            var resultado = await categoriasOrdenadas.ToPagedListAsync(categoriaParams.PageNumber, categoriaParams.PageSize);

            return resultado;
        }

        public async Task<IPagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriaFiltroParams)
        {
            var categorias = await GetAllAsync();

            if (!string.IsNullOrEmpty(categoriaFiltroParams.Nome))
            {
                categorias = categorias.Where(c => c.Nome.Contains(categoriaFiltroParams.Nome));
            }

            //var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(), categoriaFiltroParams.PageNumber, categoriaFiltroParams.PageSize);
            var categoriasFiltradas = await categorias.ToPagedListAsync(categoriaFiltroParams.PageNumber, categoriaFiltroParams.PageSize);

            return categoriasFiltradas;
        }
    }
}
