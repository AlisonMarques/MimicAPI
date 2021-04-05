using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
       public ActionResult ObterTodasPalavras(DateTime? date)
       {
           var item = _banco.Palavras.AsQueryable();
            // Verificando se data tem valor
            if (date.HasValue)
            {
                // verificando se a data do download é maior que a data da nova atualização
                item = item.Where(a => a.Criado > date.Value || a.Atualizado > date.Value);
            }
            // return new JsonResult(_banco.Palavras); ou o método abaixo
            return Ok(item);
        }
        
        //exemplo: /api/palavras/1
        [Route("{id}")]
        [HttpGet] 
        //Pegar apenas uma palavra
        public ActionResult ObterUmaPalavra(int id)
        {
            //Verificando se a palavra existe pelo id
            var obj = _banco.Palavras.Find(id);
            // se nao existe, retornar um not found 404
            if (obj == null)
                return NotFound();
            
            return Ok(obj);
        }
        
        [Route("")]
        [HttpPost] 
        //Cadastrar palavra
        public ActionResult CadastrarPalavra([FromBody]Palavra palavra)
        {
            // adicionando a palavra ao banco de dados
             _banco.Palavras.Add(palavra);
             
             //Salvando a palavra no banco de dados
             _banco.SaveChanges();
             return Created($"/api/palavras/{palavra.Id}", palavra);
        }
        
        [Route("{id}")]
        [HttpPut]
        //Atualizar palavra
        public ActionResult AtualizarPalavra(int id, [FromBody]Palavra palavra)
        {

            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);

            if (obj == null)
                return NotFound();
            
            palavra.Id = id;

            //Atualizando a palavra no banco de dados
            _banco.Palavras.Update(palavra);
            
            //Salvando a palavra atualizada no banco de dados
            _banco.SaveChanges();
            return Ok();
        }
        
        [Route("{id}")]
        [HttpDelete] 
        // Deletar palavra
        public ActionResult DeletarPalavra(int id)
        {
            var palavra = _banco.Palavras.Find(id);

            if (palavra == null)
                return NotFound();

            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
            /*_banco.Palavras.Remove(_banco.Palavras.Find(id));*/
            return NoContent();
        }
        
    }
}