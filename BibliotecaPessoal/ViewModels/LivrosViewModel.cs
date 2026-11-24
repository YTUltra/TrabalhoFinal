using System.Collections.ObjectModel;
using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;
using BibliotecaPessoal.Views;

namespace BibliotecaPessoal.ViewModels;

public class LivrosViewModel : BaseViewModel
{
    private readonly BibliotecaService _biblioteca;
    private List<Livro> _todos = new();
    private string _pesquisa = "";

    public ObservableCollection<Livro> Livros { get; } = new();
    public string Pesquisa { get => _pesquisa; set { _pesquisa = value; Filtrar(); } }
    public Command AtualizarCommand { get; }
    public Command AdicionarCommand { get; }
    public Command<Livro> EditarCommand { get; }
    public Command<Livro> ExcluirCommand { get; }

    public LivrosViewModel(BibliotecaService biblioteca)
    {
        _biblioteca = biblioteca;
        AtualizarCommand = new Command(async () => await Atualizar());
        AdicionarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(LivroFormPage)));
        EditarCommand = new Command<Livro>(async livro =>
        {
            if (livro is not null) await Shell.Current.GoToAsync($"{nameof(LivroFormPage)}?id={Uri.EscapeDataString(livro.Id)}");
        });
        ExcluirCommand = new Command<Livro>(async livro => await Excluir(livro));
    }

    public async Task Atualizar()
    {
        try
        {
            _todos = await _biblioteca.ListarLivros();
            Filtrar();
        }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }

    private void Filtrar()
    {
        var texto = Pesquisa.Trim();
        var filtrados = string.IsNullOrWhiteSpace(texto) ? _todos : _todos.Where(l =>
            l.Titulo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
            l.Autor.Contains(texto, StringComparison.OrdinalIgnoreCase));
        Livros.Clear();
        foreach (var livro in filtrados.OrderBy(l => l.Titulo)) Livros.Add(livro);
        Avisar(nameof(Livros));
    }

    private async Task Excluir(Livro? livro)
    {
        if (livro is null) return;
        var confirmar = await Shell.Current.DisplayAlertAsync("Excluir livro", $"Deseja excluir '{livro.Titulo}'?", "Excluir", "Cancelar");
        if (!confirmar) return;
        try { await _biblioteca.ExcluirLivro(livro.Id); await Atualizar(); await MensagemService.Mostrar("Livro excluído."); }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }
}
