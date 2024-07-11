using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories
{
    public interface IProdutoRepository
    {
        IQueryable<Produto> GetProdutos();
        Produto GetProdutoById(int id);
        Produto CreateProduto(Produto produto);
        bool UpdateProduto(Produto produto);
        bool DeleteProduto(int id);
    }
}
