﻿using ApiCatalogo.Controllers;
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
    public class PostProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
    {
        private readonly ProdutosController _controller;
        public PostProdutoUnitTests(ProdutosUnitTestController controller)
        {
            _controller = new ProdutosController(controller.repository, controller.mapper);
        }

        [Fact]
        public async Task PostProduto_Return_CreatedStatusCode()
        {
            //Arrange
            var novoProdutoDTO = new ProdutoDTO
            {
                Nome = "NovoProduto",
                Descricao = "Descrição do Novo Produto",
                Preco = 10.99m,
                ImagemUrl = "imagemfake1.jpg",
                CategoriaId = 2
            };

            //Act
            var data = await _controller.Post(novoProdutoDTO);

            //Assert
            var createdResult = data.Result.Should().BeOfType<CreatedAtRouteResult>();
            createdResult.Subject.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task PostProduto_Return_BadRequest()
        {
            //Arrange
            ProdutoDTO prod = null;

            //Act
            var data = await _controller.Post(prod);

            //Assert
            var createdResult = data.Result.Should().BeOfType<BadRequestResult>();
            createdResult.Subject.StatusCode.Should().Be(400);
        }
    }
}
