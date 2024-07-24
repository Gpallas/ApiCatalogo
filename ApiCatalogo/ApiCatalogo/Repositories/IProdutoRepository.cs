using ApiCatalogo.Models;
using ApiCatalogo.Pagination;
using X.PagedList;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        //IEnumerable<Produto> GetProdutos(ProdutosParameters produtoParams);
        Task<IPagedList<Produto>> GetProdutosAsync(ProdutosParameters produtoParams);
        Task<IEnumerable<Produto>> GetProdutosPorCategoriaAsync(int categoriaId);
        Task<IPagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParams);
    }
}
