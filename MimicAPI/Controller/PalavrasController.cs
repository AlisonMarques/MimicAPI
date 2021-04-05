using Microsoft.AspNetCore.Mvc;
using MimicAPI.Database;
using MimicAPI.Models;

namespace MimicAPI.Controller
{
    //Controller é um pacote mais usado/focada em criação de sites
    //ControllerBase se usa mais na criação de API (mais indicado)
    
    //Criando a rota que vai ser usado para utilizar todos os métodos do controller palavra
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        //Construtor
        public PalavrasController(MimicContext banco)
        {
            //injeção de dependencia
            _banco = banco;
        }
        //Criando a rota do método ObterTodasPalavras()
        [Route("")]
        [HttpGet] 
       //opção de obter todas as palavras (vai ser usado no botão de atualizar palavras no aplicativo)
       public ActionResult ObterTodasPalavras()
        {
            // return new JsonResult(_banco.Palavras); ou o método abaixo
            return Ok(_banco.Palavras);
        }
        
        //exemplo: /api/palavras/1
        [Route("{id}")]
        [HttpGet] 
        //Pegar apenas uma palavra
        public ActionResult ObterUmaPalavra(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }
        
        [Route("")]
        [HttpPost] 
        //Cadastrar palavra
        public ActionResult CadastrarPalavra(Palavra palavra)
        {
             _banco.Palavras.Add(palavra);
             return Ok();
        }
        
        [Route("{id}")]
        [HttpPut]
        //Atualizar palavra
        public ActionResult AtualizarPalavra(int id, Palavra palavra)
        {
            palavra.Id = id;
            _banco.Palavras.Update(palavra);
            return Ok();
        }
        
        [Route("{id}")]
        [HttpDelete] 
        // Deletar palavra
        public ActionResult DeletarPalavra(int id)
        {
            _banco.Palavras.Remove(_banco.Palavras.Find(id));
            return Ok();
        }
        
    }
}