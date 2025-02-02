﻿using ApiCatalogo.Context;
using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories
{
    public class SpecificProdutoRepository : ISpecificProdutoRepository
    {
        private readonly AppDbContext _context;

        public SpecificProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Produto> GetProdutos()
        {
            return _context.Produtos;
        }

        public Produto GetProdutoById(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            if (produto is null)
            {
                throw new InvalidOperationException("Produto é null");
            }

            return produto;
        }

        public Produto CreateProduto(Produto produto)
        {
            if (produto is null)
            {
                throw new InvalidOperationException("Produto é null");
            }

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return produto;
        }

        public bool UpdateProduto(Produto produto)
        {
            if (produto is null)
            {
                throw new InvalidOperationException("Produto é null");
            }

            if (_context.Produtos.Any(p => p.ProdutoId == produto.ProdutoId))
            {
                _context.Produtos.Update(produto);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        public bool DeleteProduto(int id)
        {
            var produto = _context.Produtos.Find(id);

            if (produto is not null)
            {
                _context.Remove(produto);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
