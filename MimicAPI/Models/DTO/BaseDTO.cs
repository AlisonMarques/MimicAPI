using System.Collections.Generic;

namespace MimicAPI.Models.DTO
{
    public abstract class BaseDTO
    {
        public List<LinkDTO> Links { get; set; }
    }
}