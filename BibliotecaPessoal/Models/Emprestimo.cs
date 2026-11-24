namespace BibliotecaPessoal.Models;

public class Emprestimo : Registro
{
    public string LivroId { get; set; } = "";
    public string LivroTitulo { get; set; } = "";
    public string Pessoa { get; set; } = "";
    public DateTime DataEmprestimo { get; set; } = DateTime.Today;
    public DateTime DataPrevistaDevolucao { get; set; } = DateTime.Today.AddDays(14);
    public bool Devolvido { get; set; }
}
