using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;

namespace BibliotecaPessoal.Views;

public partial class LivroFormPage : ContentPage, IQueryAttributable
{
    private readonly BibliotecaService _biblioteca;
    private string? _id;
    private bool _carregado;

    public LivroFormPage(BibliotecaService biblioteca)
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
        try
        {
            CategoriaPicker.ItemsSource = await _biblioteca.ListarCategorias();
            if (string.IsNullOrWhiteSpace(_id)) return;
            var livro = (await _biblioteca.ListarLivros()).FirstOrDefault(item => item.Id == _id);
            if (livro is null) { await DisplayAlertAsync("Aviso", "Livro não encontrado.", "OK"); return; }
            TituloPagina.Text = "Editar livro";
            TituloEntry.Text = livro.Titulo;
            AutorEntry.Text = livro.Autor;
            AnoEntry.Text = livro.AnoPublicacao.ToString();
            CategoriaPicker.SelectedItem = ((List<Categoria>)CategoriaPicker.ItemsSource).FirstOrDefault(c => c.Id == livro.CategoriaId);
            FisicoRadio.IsChecked = livro.Formato == "Físico";
            EbookRadio.IsChecked = livro.Formato == "E-book";
            AudioRadio.IsChecked = livro.Formato == "Áudio";
            LidoCheck.IsChecked = livro.Lido;
            FavoritoCheck.IsChecked = livro.Favorito;
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    private async void SalvarClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TituloEntry.Text) || string.IsNullOrWhiteSpace(AutorEntry.Text) ||
            !int.TryParse(AnoEntry.Text, out var ano) || ano < 1 || ano > DateTime.Today.Year || CategoriaPicker.SelectedItem is not Categoria categoria)
        {
            await DisplayAlertAsync("Validação", "Preencha título, autor, ano válido e categoria.", "OK");
            return;
        }

        try
        {
            var livro = new Livro
            {
                Id = _id ?? "",
                Titulo = TituloEntry.Text.Trim(),
                Autor = AutorEntry.Text.Trim(),
                AnoPublicacao = ano,
                CategoriaId = categoria.Id,
                Formato = EbookRadio.IsChecked ? "E-book" : AudioRadio.IsChecked ? "Áudio" : "Físico",
                Lido = LidoCheck.IsChecked,
                Favorito = FavoritoCheck.IsChecked,
                Capa = "capa_livro.png"
            };
            await _biblioteca.SalvarLivro(livro);
            await MensagemService.Mostrar("Livro salvo.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK"); }
    }

    private async void CancelarClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
