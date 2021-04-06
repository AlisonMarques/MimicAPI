using AutoMapper;
using MimicAPI.Models;
using MimicAPI.Models.DTO;

namespace MimicAPI.Helpers
{
    public class DTOMapperProfile : Profile
    {
        //Config profile
        public DTOMapperProfile()
        {
            // convertendo Palavra para PalavaDTo
            CreateMap<Palavra, PalavraDto>();
        }
    }
}

        /*
         * AutoMapper é uma biblioteca que vai nos ajudar
         * a copiar um objeto de um tipo para outro objeto
         */