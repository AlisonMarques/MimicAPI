using System;

namespace MimicAPI.Models.DTO
{
    public class PalavraDto : BaseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Pontuacao { get; set; }
        public bool Ativo { get; set; }

        public DateTime Criado { get; set; }
        public DateTime? Atualizado { get; set; }
    }
}