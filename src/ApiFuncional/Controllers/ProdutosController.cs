using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFuncional.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProdutosController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos() //Task de um actionResult que retorna uma coleção de Produto
        {
            return await _context.Produtos.ToListAsync(); //Retorna um Ok Result por padrão - 200
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        { 
            var produto = await _context.Produtos.FindAsync(id);

            return produto;
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (!ModelState.IsValid)//
            {
                //return BadRequest(ModelState); - Validações sem mais descrições de erro

                //return ValidationProblem(ModelState); - Validação com descrição do erro

                return ValidationProblem(new ValidationProblemDetails(ModelState)//validação com descrição do erro e mensagem personalizada
                {
                    Title = "Um ou mais erros de validação ocorreram!"
                });
            }//

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto); //CreatedAction - 201 -> pra chamar e conferir, vai no método gGet passando esse parâmetro e retorne o produto persistido
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto) 
        {
            if (id != produto.Id) return BadRequest();//Se o id informado na rota é o mesmo do objeto informado, caso não retorna ...

            if (!ModelState.IsValid) return ValidationProblem(ModelState);//Se model state for invalida, retorna..

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            return NoContent();//204
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduto(int id) 
        {
            var produto = await _context.Produtos.FindAsync(id);

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();//204
        }
    }
}
