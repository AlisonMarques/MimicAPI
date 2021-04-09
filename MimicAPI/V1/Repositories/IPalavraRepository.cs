using System.Collections.Generic;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Contracts
{
    public interface IPalavraRepository
    {
        PaginationList<Palavra> ObterTodasPalavras(PalavraUrlQuery query);

        Palavra ObterUmaPalavra(int id);

        void CadastrarPalavra(Palavra palavra);
        
        void AtualizarPalavra(Palavra palavra);

        void DeletarPalavra(int id);

    }
}