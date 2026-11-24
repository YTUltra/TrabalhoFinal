namespace BibliotecaPessoal.Models;

public class Livro : Registro
{
    public string Titulo { get; set; } = "";
    public string Autor { get; set; } = "";
    public int AnoPublicacao { get; set; }
    public string CategoriaId { get; set; } = "";
    public string Formato { get; set; } = "Físico";
    public bool Lido { get; set; }
    public bool Favorito { get; set; }
    public string Capa { get; set; } = "capa_livro.png";
}
