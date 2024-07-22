using ApiCatalogo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiCatalogo.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(20, ErrorMessage = "O nome deve ter entre 5 e 20 caractéres", MinimumLength = 5)]
        public string? Nome { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "A descrição deve ter no máximo {1} caractéres")]
        public string? Descricao { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 10)]
        public string? ImagemUrl { get; set; }

        public int CategoriaId { get; set; }
    }
}
