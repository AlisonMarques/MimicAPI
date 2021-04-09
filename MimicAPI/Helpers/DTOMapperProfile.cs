using AutoMapper;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        //Config profile
        public DTOMapperProfile()
        {
            // Criando o mapeamento
            // convertendo Palavra para PalavaDTo
            CreateMap<Palavra, PalavraDto>();
            
            CreateMap<PaginationList<Palavra>, PaginationList<PalavraDto>>();

        }
    }
}

        /*
         * AutoMapper é uma biblioteca que vai nos ajudar
         * a copiar um objeto de um tipo para outro objeto
         */