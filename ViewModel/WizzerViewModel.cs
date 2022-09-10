using System;

namespace EFVsDapperBattle.ViewModel
{
    public class WizzerViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public DateOnly DataDeNascimento { get; set; }
    }
}
