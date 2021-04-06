using System;

namespace MimicAPI.Models.DTO
{
    public class LinkDTO
    {
        public string Rel { get; set; }

        public string Href { get; set; }

        public string Method { get; set; }

        public LinkDTO(string rel, string href, string method)
        {
            Rel = rel;
            Href = href;
            Method = method;
        }
    }
}