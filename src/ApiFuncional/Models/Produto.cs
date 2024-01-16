using System.ComponentModel.DataAnnotations;

namespace ApiFuncional.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigratório")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigratório")]
        [Range(1, int.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigratório")]
        public int QUantidadeEstoque { get; set; }

        [Required(ErrorMessage = "O campo {0} é Obrigratório")]
        public string? Descricao { get; set; }
    }
}
