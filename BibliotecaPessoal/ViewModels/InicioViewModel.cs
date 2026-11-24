using BibliotecaPessoal.Services;

namespace BibliotecaPessoal.ViewModels;

public class InicioViewModel : BaseViewModel
{
    private readonly BibliotecaService _biblioteca;
    private string _resumo = "Carregue os dados para ver um resumo.";

    public string Resumo
    {
        get => _resumo;
        set { _resumo = value; Avisar(); }
    }

    public Command AtualizarCommand { get; }
    public Command AbrirLivrosCommand { get; }
    public Command AbrirCategoriasCommand { get; }
    public Command AbrirEmprestimosCommand { get; }

    public InicioViewModel(BibliotecaService biblioteca)
    {
        _biblioteca = biblioteca;
        AtualizarCommand = new Command(async () => await Atualizar());
        AbrirLivrosCommand = new Command(async () => await Shell.Current.GoToAsync("//biblioteca/aba-livros/livros"));
        AbrirCategoriasCommand = new Command(async () => await Shell.Current.GoToAsync("//biblioteca/aba-categorias/categorias"));
        AbrirEmprestimosCommand = new Command(async () => await Shell.Current.GoToAsync("//biblioteca/aba-emprestimos/emprestimos"));
    }

    public async Task Atualizar()
    {
        try
        {
            var livros = await _biblioteca.ListarLivros();
            var categorias = await _biblioteca.ListarCategorias();
            var emprestimos = await _biblioteca.ListarEmprestimos();
            var emprestados = emprestimos.Count(e => !e.Devolvido);
            Resumo = $"{livros.Count} livros · {categorias.Count} categorias · {emprestados} emprestados";
        }
        catch (Exception ex)
        {
            Resumo = ex.Message;
        }
    }
}
