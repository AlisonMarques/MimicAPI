using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;

namespace MimicAPI.Repositories.Contracts.Contracts
{
    public class PalavraRepository : IPalavraRepository
    {
        //construtor
        private readonly MimicContext _banco;
        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }
        
        public PaginationList<Palavra> ObterTodasPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();
            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            // Verificando se data tem valor
            if (query.Date.HasValue)
            {
                // verificando se a data do download é maior que a data da nova atualização
                item = item.Where(a => a.Criado > query.Date.Value || a.Atualizado > query.Date.Value);
            }
            
            //Criando também a paginação
            // skip é pular e take é pegar
            if (query.PagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);

                // Desenvolvendo a lógica da paginação
                var pagination = new Pagination();
                pagination.NumeroPagina = query.PagNumero.Value; 
                pagination.RegistroPorPagina = query.PagRegistro.Value;
                pagination.TotalRegistros = quantidadeTotalRegistros;
                pagination.TotalPaginas = (int) Math.Ceiling((double) quantidadeTotalRegistros / query.PagRegistro.Value);

                lista.Pagination = pagination;
            }
            lista.AddRange(item.ToList());

            return lista;
        }

        public Palavra ObterUmaPalavra(int id)
        {
           return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void CadastrarPalavra(Palavra palavra)
        {
            // adicionando a palavra ao banco de dados
            _banco.Palavras.Add(palavra);
             
            //Salvando a palavra no banco de dados
            _banco.SaveChanges();
        }

        public void AtualizarPalavra(Palavra palavra)
        {
            //Atualizando a palavra no banco de dados
            _banco.Palavras.Update(palavra);
            
            //Salvando a palavra atualizada no banco de dados
            _banco.SaveChanges();
        }

        public void DeletarPalavra(int id)
        {
            var palavra = ObterUmaPalavra(id);
            
            palavra.Ativo = false;

            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
    }
}