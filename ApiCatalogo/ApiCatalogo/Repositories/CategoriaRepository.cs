using ApiCatalogo.Context;
using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public PagedList<Categoria> GetCategorias(CategoriasParameters categoriaParams)
        {
            var categorias = GetAll().OrderBy(c => c.CategoriaId).AsQueryable();
            var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categorias, categoriaParams.PageNumber, categoriaParams.PageSize);

            return categoriasOrdenadas;
        }

        public PagedList<Categoria> GetCategoriasFiltroNome(CategoriasFiltroNome categoriaFiltroParams)
        {
            var categorias = GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(categoriaFiltroParams.Nome))
            {
                categorias = categorias.Where(c => c.Nome.Contains(categoriaFiltroParams.Nome));
            }

            var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias, categoriaFiltroParams.PageNumber, categoriaFiltroParams.PageSize);

            return categoriasFiltradas;
        }
    }
}
