using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        //IEnumerable<Produto> GetProdutos(ProdutosParameters produtoParams);
        Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtoParams);
        Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId);
        Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams);
    }
}
