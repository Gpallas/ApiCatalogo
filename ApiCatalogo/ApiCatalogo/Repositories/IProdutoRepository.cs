using ApiCatalogo.Models;
using ApiCatalogo.Pagination;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        //IEnumerable<Produto> GetProdutos(ProdutosParameters produtoParams);
        PagedList<Produto> GetProdutos(ProdutosParameters produtoParams);
        IEnumerable<Produto> GetProdutosPorCategoria(int categoriaId);
    }
}
