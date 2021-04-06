using System.Threading;

namespace MimicAPI.Helpers
{
    public class Pagination
    {
        public int NumeroPagina { get; set; }
        public int RegistroPorPagina { get; set; }

        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}