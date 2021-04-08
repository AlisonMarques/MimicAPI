    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MimicAPI.Helpers;
    using MimicAPI.Models;
    using System.Text.Json;
    using AutoMapper;
    using MimicAPI.Models.DTO;
    using MimicAPI.Repositories.Contracts;

    namespace MimicAPI.Controller
    {
        //Controller é um pacote mais usado/focada em criação de sites
        //ControllerBase se usa mais na criação de API (mais indicado)
        
        //Criando a rota que vai ser usado para utilizar todos os métodos do controller palavra
        [Route("api/palavras")]
        public class PalavrasController : ControllerBase
        {        
            
            private readonly IPalavraRepository _repository;
            private readonly IMapper _mapper;
            //Construtor
            public PalavrasController(IPalavraRepository repository, IMapper mapper)
            {
                //injeção de dependencia
                _repository = repository;
                _mapper = mapper;
            }
            //Criando a rota do método ObterTodasPalavras()
            /*[Route("")]*/
            [HttpGet("", Name = "ObterTodasPalavras")] 
           //opção de obter todas as palavras (vai ser usado no botão de atualizar palavras no aplicativo)
           public ActionResult ObterTodasPalavras([FromQuery]PalavraUrlQuery query)
           {
               var item = _repository.ObterTodasPalavras(query);
               
               if (item.Results.Count == 0)
                   return NotFound();
               
               
               var lista = CriarLinksListaPalavra(query, item);
               // return new JsonResult(_banco.Palavras); ou o método abaixo
                return Ok(lista);
            }

           private PaginationList<PalavraDto> CriarLinksListaPalavra(PalavraUrlQuery query, PaginationList<Palavra> item)
           {
               // Utilizando o autoMapper para converter em PalavraDto e utilizar os links dinâmicos
               var lista = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDto>>(item);

               // Criando links para cada palavra
               foreach (var palavra in lista.Results)
               {
                   palavra.Links = new List<LinkDTO>();
                   palavra.Links.Add(new LinkDTO("self", Url.Link("ObterUmaPalavra", new {id = palavra.Id}), "GET"));
               }

               lista.Links.Add(new LinkDTO("self", Url.Link("ObterTodasPalavras", query), "GET"));

               if (item.Pagination != null)
               {
                   //Criando X-Pagination no header da api com as informações do objeto pagination
                   Response.Headers.Add("X-Pagination",
                       JsonSerializer.Serialize(item.Pagination));

                   // Verificando se existe uma próxima página ou página anterio
                   // Lógica da proxima página
                   if (query.PagNumero + 1 <= item.Pagination.TotalPaginas)
                   {
                       var queryString = new PalavraUrlQuery()
                           {PagNumero = query.PagNumero + 1, PagRegistro = query.PagRegistro};

                       lista.Links.Add(new LinkDTO("next", Url.Link("ObterTodasPalavras", queryString), "GET"));
                   }

                   // Lógica da página anterior
                   if (query.PagNumero - 1 > 0)
                   {
                       var queryString = new PalavraUrlQuery()
                           {PagNumero = query.PagNumero - 1, PagRegistro = query.PagRegistro};
                       lista.Links.Add(new LinkDTO("prev", Url.Link("ObterTodasPalavras", queryString), "GET"));
                   }
               }

               return lista;
           }

           //exemplo: /api/palavras/1
            /*[Route("{id}")] Removido pois ao adicionar o Url.Link ele passa o id como query e não é isso que queremos
             para resolver isso vamos passar o parâmetro dentro do HttpGet
             */
            [HttpGet("{id}", Name = "ObterUmaPalavra")] 
            //Pegar apenas uma palavra
            public ActionResult ObterUmaPalavra(int id)
            {
                var obj = _repository.ObterUmaPalavra(id);
                
                //Verificando se a palavra existe pelo id
                // se nao existe, retornar um not found 404
                if (obj == null)
                    return NotFound();

                //Mapeando a Palavra e transformando em PalavraDto
                PalavraDto palavraDto = _mapper.Map<Palavra, PalavraDto>(obj);
                
                //Adicionando os Links
                palavraDto.Links = new List<LinkDTO>();
                //Vantagem do Url.Link é deixar o link da api dinâmico e assim vai funcionar bem no deploy
                palavraDto.Links.Add(new LinkDTO("self", Url.Link("ObterUmaPalavra", new { id = palavraDto.Id}), "GET"));
                
                //Mostrando ao usuário que existe outras funcionalidades atráves dos links
                palavraDto.Links.Add(new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDto.Id}), "PUT"));
                palavraDto.Links.Add(new LinkDTO("delete", Url.Link("DeletarPalavra", new { id = palavraDto.Id}), "DELETE"));

                return Ok(palavraDto);
            }
            
            [Route("")]
            [HttpPost] 
            //Cadastrar palavra
            public ActionResult CadastrarPalavra([FromBody]Palavra palavra)
            {
                //Cadastrando palavra
                _repository.CadastrarPalavra(palavra);
                
                 return Created($"/api/palavras/{palavra.Id}", palavra);
            }
            
            /*[Route("{id}")]*/
            [HttpPut("{id}", Name = "AtualizarPalavra")]
            //Atualizar palavra
            public ActionResult AtualizarPalavra(int id, [FromBody]Palavra palavra)
            {

                var obj = _repository.ObterUmaPalavra(id);

                if (obj == null)
                    return NotFound();
                
                palavra.Id = id;
                
                _repository.AtualizarPalavra(palavra);

                return Ok();
            }
            
            /*[Route("{id}")]*/
            [HttpDelete("{id}", Name = "DeletarPalavra")] 
            // Deletar palavra
            public ActionResult DeletarPalavra(int id)
            {
                //buscando a palavra pelo id
                var palavra = _repository.ObterUmaPalavra(id);

                //Verificando se existe a palavra
                if (palavra == null)
                    return NotFound();

                _repository.DeletarPalavra(id);
                /*_banco.Palavras.Remove(_banco.Palavras.Find(id));*/
                return NoContent();
            }
            
        }
    }