using System.Collections.ObjectModel;
using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;
using BibliotecaPessoal.Views;

namespace BibliotecaPessoal.ViewModels;

public class CategoriasViewModel : BaseViewModel
{
    private readonly BibliotecaService _biblioteca;
    private List<Categoria> _todas = new();
    private string _pesquisa = "";

    public ObservableCollection<Categoria> Categorias { get; } = new();
    public string Pesquisa { get => _pesquisa; set { _pesquisa = value; Filtrar(); } }
    public Command AtualizarCommand { get; }
    public Command AdicionarCommand { get; }
    public Command<Categoria> EditarCommand { get; }
    public Command<Categoria> ExcluirCommand { get; }

    public CategoriasViewModel(BibliotecaService biblioteca)
    {
        _biblioteca = biblioteca;
        AtualizarCommand = new Command(async () => await Atualizar());
        AdicionarCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(CategoriaFormPage)));
        EditarCommand = new Command<Categoria>(async categoria =>
        {
            if (categoria is not null) await Shell.Current.GoToAsync($"{nameof(CategoriaFormPage)}?id={Uri.EscapeDataString(categoria.Id)}");
        });
        ExcluirCommand = new Command<Categoria>(async categoria => await Excluir(categoria));
    }

    public async Task Atualizar()
    {
        try { _todas = await _biblioteca.ListarCategorias(); Filtrar(); }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }

    private void Filtrar()
    {
        var texto = Pesquisa.Trim();
        var filtradas = string.IsNullOrWhiteSpace(texto) ? _todas : _todas.Where(c =>
            c.Nome.Contains(texto, StringComparison.OrdinalIgnoreCase) || c.Descricao.Contains(texto, StringComparison.OrdinalIgnoreCase));
        Categorias.Clear();
        foreach (var categoria in filtradas.OrderBy(c => c.Nome)) Categorias.Add(categoria);
        Avisar(nameof(Categorias));
    }

    private async Task Excluir(Categoria? categoria)
    {
        if (categoria is null) return;
        var confirmar = await Shell.Current.DisplayAlertAsync("Excluir categoria", $"Deseja excluir '{categoria.Nome}'?", "Excluir", "Cancelar");
        if (!confirmar) return;
        try { await _biblioteca.ExcluirCategoria(categoria.Id); await Atualizar(); await MensagemService.Mostrar("Categoria excluída."); }
        catch (Exception ex) { await MensagemService.Mostrar(ex.Message); }
    }
}
