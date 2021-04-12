    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using MimicAPI.Helpers;
    using MimicAPI.V1.Models;
    using System.Text.Json;
    using AutoMapper;
    using MimicAPI.V1.Models.DTO;
    using MimicAPI.V1.Repositories.Contracts;

    namespace MimicAPI.V1.Controller
    {
        //Controller é um pacote mais usado/focada em criação de sites
        //ControllerBase se usa mais na criação de API (mais indicado)
        
        // ApiController vai ajudar o versionamento funcionar
        [ApiController]
        //Criando a rota que vai ser usado para utilizar todos os métodos do controller palavra
        [Route("api/v{version:apiVersion}/palavras")]
        [ApiVersion("1.0")]
        [ApiVersion("1.1")]
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
            
            /// <summary>
            /// Operação que pega do banco de dados todas as palavras existentes.
            /// </summary>
            /// <param name="query">Filtros de pesquisa</param>
            /// <returns>Listagem de palavras</returns>
            [HttpGet("", Name = "ObterTodasPalavras")] 
            [MapToApiVersion("1.0")]
            [MapToApiVersion("1.1")]
            public ActionResult ObterTodasPalavras([FromQuery]PalavraUrlQuery query)
           {
               var item = _repository.ObterTodasPalavras(query);
               
               if (item.Results.Count == 0)
                   return NotFound();
               
               
               var lista = CriarLinksListaPalavra(query, item);
               // return new JsonResult(_banco.Palavras); ou o método abaixo
                return Ok(lista);
            }
            
            /// <summary>
            /// Operação que pega uma única palavra do banco de dados 
            /// </summary>
            /// <param name="id">Código identificado da palavra</param>
            /// <returns>Um objeto de palavra</returns>
            [HttpGet("{id}", Name = "ObterUmaPalavra")] 
            [MapToApiVersion("1.0")]
            [MapToApiVersion("1.1")]
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

                #region Adicionando os Links
                //Vantagem do Url.Link é deixar o link da api dinâmico e assim vai funcionar bem no deploy
                palavraDto.Links.Add(new LinkDTO("self", Url.Link("ObterUmaPalavra", new { id = palavraDto.Id}), "GET"));
                
                //Mostrando ao usuário que existe outras funcionalidades atráves dos links
                palavraDto.Links.Add(new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDto.Id}), "PUT"));
                palavraDto.Links.Add(new LinkDTO("delete", Url.Link("DeletarPalavra", new { id = palavraDto.Id}), "DELETE"));
                
                #endregion 
               
                
                return Ok(palavraDto);
            }
            
            /// <summary>
            /// Operação que realiza o cadastro da palavra
            /// </summary>
            /// <param name="palavra">Um objeto palavra</param>
            /// <returns>Um objeto palavra com seus atributos e id</returns>
            [Route("")]
            [HttpPost] 
            [MapToApiVersion("1.0")]
            [MapToApiVersion("1.1")]
            //Cadastrar palavra
            public ActionResult CadastrarPalavra([FromBody]Palavra palavra)
            {
                #region Validações de cadastro
                // caso o usuário enviar um formulário vazio
                if (palavra == null)
                    return BadRequest();
                
                // Validando os dados enviados. Se ficou algum campo sem preencher
                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }
                
                #endregion
                // Colocando a própria API para fornecer a data de cadastro e de Ativo
                palavra.Ativo = true;
                palavra.Criado = DateTime.Now;
                
                //Cadastrando palavra
                _repository.CadastrarPalavra(palavra);

                PalavraDto palavraDto = _mapper.Map<Palavra, PalavraDto>(palavra);
                palavraDto.Links.Add(new LinkDTO("self", 
                    Url.Link("ObterUmaPalavra", 
                        new { id = palavraDto.Id}), "GET"));
                
                 return Created($"/api/palavras/{palavra.Id}", palavraDto);
            }
            
            /// <summary>
            /// Operação que realiza a substituição de dados de uma palavra específica.
            /// </summary>
            /// <param name="id">Código identificado da palavra a ser alterada</param>
            /// <param name="palavra">Objeto palavra com dados para alteração</param>
            /// <returns>Retorna apenas a repostas http 200</returns>
            [HttpPut("{id}", Name = "AtualizarPalavra")]
            [MapToApiVersion("1.0")]
            [MapToApiVersion("1.1")]
            //Atualizar palavra
            public ActionResult AtualizarPalavra(int id, [FromBody]Palavra palavra)
            {
                #region Validações de cadastro
                var obj = _repository.ObterUmaPalavra(id);
                // validando se a palavra existe no banco de dados
                if (obj == null)
                    return NotFound();
                
                // caso o usuário enviar um formulário vazio
                if (palavra == null)
                    return BadRequest();
                
                // Validando os dados enviados. Se ficou algum campo sem preencher
                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityResult();
                }
                
                #endregion
                
                palavra.Id = id;
                
                //Setando automáticamente esses valores para que o usuário não possa modificar.
                palavra.Ativo = obj.Ativo;
                palavra.Criado = obj.Criado;
                
                // API colocará a data sozinha // controle interno
                palavra.Atualizado = DateTime.Now;
                
                _repository.AtualizarPalavra(palavra);

                PalavraDto palavraDto = _mapper.Map<Palavra, PalavraDto>(palavra);
                // Passando o link para poder ver a palavra atualizada
                palavraDto.Links.Add(new LinkDTO("self", 
                    Url.Link("ObterUmaPalavra", 
                        new { id = palavraDto.Id}), "GET"));

                return Ok();
            }
            
            /// <summary>
            /// Operação que desativa uma palavra do sistema.
            /// </summary>
            /// <param name="id">Código identificado da palavra</param>
            /// <returns>Retorna apenas a repostas http 204</returns>
            [HttpDelete("{id}", Name = "DeletarPalavra")] 
            [MapToApiVersion("1.0")]
            [MapToApiVersion("1.1")]
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

        }
    }