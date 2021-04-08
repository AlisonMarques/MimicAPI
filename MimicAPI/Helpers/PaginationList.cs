using System.Collections.Generic;
using MimicAPI.Models.DTO;

namespace MimicAPI.Helpers
{
    public class PaginationList<T>
    {
        public List<T> Results { get; set; } = new List<T>();
        public Pagination Pagination { get; set; }

        // será os links de paginação para ir para próxima page ou voltar
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}