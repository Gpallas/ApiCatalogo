using ApiCatalogo.Controllers;
using ApiCatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogoxUnitTests.UnitTests
{
    public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;
        public PutProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PutProduto_Update_Return_OkResult()
        {
            //Arrange
            var prodId = 8;

            var updatedProdutoDTO = new ProdutoDTO
            {
                ProdutoId = prodId,
                Nome = "Produto Atualizado",
                Descricao = "Minha Descrição",
                ImagemUrl = "imagem1.jpg",
                CategoriaId = 2
            };

            //Act
            var result = await _controller.Put(prodId, updatedProdutoDTO) as ActionResult<ProdutoDTO>;

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PutProduto_Update_Return_BadRequest()
        {
            //Arrange
            var prodId = 999;

            var updatedProdutoDTO = new ProdutoDTO
            {
                ProdutoId = 8,
                Nome = "Produto Atualizado",
                Descricao = "Minha Descrição",
                ImagemUrl = "imagem1.jpg",
                CategoriaId = 2
            };

            //Act
            var result = await _controller.Put(prodId, updatedProdutoDTO);

            //Assert
            result.Result.Should().BeOfType<BadRequestResult>().Which.StatusCode.Should().Be(400);
        }
    }
}
