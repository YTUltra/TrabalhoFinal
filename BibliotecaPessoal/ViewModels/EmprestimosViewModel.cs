using System.Collections.ObjectModel;
using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;
using BibliotecaPessoal.Views;

namespace BibliotecaPessoal.ViewModels;

public class EmprestimosViewModel : BaseViewModel
{
    private readonly BibliotecaService _biblioteca;
    private List<Emprestimo> _todos = new();
    private string _pesquisa = "";

    public ObservableCollection<Emprestimo> Emprestimos { get; } = new();
    public string Pesquisa { get => _pesquisa; set { _pesquisa = value; Filtrar(); } }
    public Command AtualizarCommand { get; }
    public Command AdicionarCommand { get; }
    public Command<Emprestimo> EditarCommand { get; }
    public Command<Emprestimo> ExcluirCommand { get; }

    public EmprestimosViewModel(BibliotecaService biblioteca)
    {
        _biblioteca = biblioteca;
        AtualizarCommand = new Command(async () => await Atualizar());
        AdicionarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(EmprestimoFormPage)));
        EditarCommand = new Command<Emprestimo>(async emprestimo =>
        {
            if (emprestimo is not null) await Shell.Current.GoToAsync($"{nameof(EmprestimoFormPage)}?id={Uri.EscapeDataString(emprestimo.Id)}");
        });
        ExcluirCommand = new Command<Emprestimo>(async emprestimo => await Excluir(emprestimo));
    }

    public async Task Atualizar()
    {
        try { _todos = await _biblioteca.ListarEmprestimos(); Filtrar(); }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }

    private void Filtrar()
    {
        var texto = Pesquisa.Trim();
        var filtrados = string.IsNullOrWhiteSpace(texto) ? _todos : _todos.Where(e =>
            e.Pessoa.Contains(texto, StringComparison.OrdinalIgnoreCase) || e.LivroTitulo.Contains(texto, StringComparison.OrdinalIgnoreCase));
        Emprestimos.Clear();
        foreach (var emprestimo in filtrados.OrderByDescending(e => e.DataEmprestimo)) Emprestimos.Add(emprestimo);
        Avisar(nameof(Emprestimos));
    }

    private async Task Excluir(Emprestimo? emprestimo)
    {
        if (emprestimo is null) return;
        var confirmar = await Shell.Current.DisplayAlertAsync("Excluir empréstimo", "Deseja excluir este registro?", "Excluir", "Cancelar");
        if (!confirmar) return;
        try { await _biblioteca.ExcluirEmprestimo(emprestimo.Id); await Atualizar(); await MensagemService.Mostrar("Empréstimo excluído."); }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }
}
