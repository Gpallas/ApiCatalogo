﻿using ApiCatalogo.Models;

namespace ApiCatalogo.Repositories
{
    public interface ICategoriaRepository
    {
        IEnumerable<Categoria> GetCategorias();
        Categoria GetCategoriaById(int id);
        Categoria CreateCategoria(Categoria categoria);
        Categoria UpdateCategoria(Categoria categoria);
        Categoria DeleteCategoriaById(int id);
    }
}
