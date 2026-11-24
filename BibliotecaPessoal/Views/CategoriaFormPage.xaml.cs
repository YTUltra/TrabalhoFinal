using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;

namespace BibliotecaPessoal.Views;

public partial class CategoriaFormPage : ContentPage, IQueryAttributable
{
    private readonly BibliotecaService _biblioteca;
    private string? _id;
    private bool _carregado;

    public CategoriaFormPage(BibliotecaService biblioteca)
    {
        InitializeComponent();
        _biblioteca = biblioteca;
        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _id = query.TryGetValue("id", out var id) ? id?.ToString() : null;
        _carregado = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_carregado) return;
        _carregado = true;
        if (string.IsNullOrWhiteSpace(_id)) return;
        try
        {
            var categoria = (await _biblioteca.ListarCategorias()).FirstOrDefault(item => item.Id == _id);
            if (categoria is null) { await DisplayAlertAsync("Aviso", "Categoria não encontrada.", "OK"); return; }
            TituloPagina.Text = "Editar categoria";
            NomeEntry.Text = categoria.Nome;
            DescricaoEditor.Text = categoria.Descricao;
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    private async void SalvarClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text))
        {
            await DisplayAlertAsync("Validação", "Informe o nome da categoria.", "OK");
            return;
        }
        try
        {
            await _biblioteca.SalvarCategoria(new Categoria
            {
                Id = _id ?? "",
                Nome = NomeEntry.Text.Trim(),
                Descricao = DescricaoEditor.Text?.Trim() ?? ""
            });
            await MensagemService.Mostrar("Categoria salva.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK"); }
    }

    private async void CancelarClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
