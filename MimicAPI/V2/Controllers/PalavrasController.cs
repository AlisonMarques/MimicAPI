using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2.Controllers
{
    // /api/v2.0/palavras
    
    [ApiController]
    [Route("api/v{version:apiVersion}/palavras")]
    [ApiVersion("2.0")]
    public class PalavraController : ControllerBase
    {
        [HttpGet("", Name = "ObterTodasPalavras")]
        public string ObterTodasPalavras()
        {
            return "Versão 2.0";
        }
    }
}