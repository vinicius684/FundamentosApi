using ApiFuncional.Data;
using ApiFuncional.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiFuncional.Controllers
{
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]//Formatador de resposta
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos() //Task de um actionResult que retorna uma coleção de Produto
        {
            if (_context.Produtos == null)//tratamento
            {
                return NotFound();
            }

            return await _context.Produtos.ToListAsync(); //Retorna um Ok Result por padrão - 200
        }

        [AllowAnonymous]
        //[EnableCors("Production")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (_context.Produtos == null)//tratamento
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)//tratamento
            {
                return NotFound();
            }

            return produto;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (_context.Produtos == null)//tratamento
            {
                return Problem("Erro ao criar um produto, contate o suporte!");//poderia colcoar um log de erro
            }

            if (!ModelState.IsValid)//validação
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutProduto(int id, Produto produto) 
        {
            if (id != produto.Id) return BadRequest();//Se o id informado na rota é o mesmo do objeto informado, caso não retorna ...

            if (!ModelState.IsValid) return ValidationProblem(ModelState);//Se model state for invalida, retorna..

            _context.Entry(produto).State = EntityState.Modified;//update com tratamento de erro p/ erro do ef core, atachamento de memória

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)//tratamento de concorriência no banco 
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //_context.Produtos.Update(produto);
            //await _context.SaveChangesAsync();

            return NoContent();//204
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteProduto(int id) 
        {
            if (_context.Produtos == null)//tratamento
            {
                return NotFound();
            }
            
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)//tratamento
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();//204
        }
        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
