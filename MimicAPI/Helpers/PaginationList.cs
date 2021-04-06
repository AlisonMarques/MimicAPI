using System.Collections.Generic;

namespace MimicAPI.Helpers
{
    public class PaginationList<T> : List<T>
    {
        public Pagination Pagination { get; set; }
    }
}