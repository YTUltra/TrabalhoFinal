using BibliotecaPessoal.Models;
using BibliotecaPessoal.Services;

namespace BibliotecaPessoal.Views;

public partial class EmprestimoFormPage : ContentPage, IQueryAttributable
{
    private readonly BibliotecaService _biblioteca;
    private string? _id;
    private bool _carregado;

    public EmprestimoFormPage(BibliotecaService biblioteca)
    {
        InitializeComponent();
        _biblioteca = biblioteca;
        DataEmprestimoPicker.Date = DateTime.Today;
        DataDevolucaoPicker.Date = DateTime.Today.AddDays(14);
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
            LivroPicker.ItemsSource = (await _biblioteca.ListarLivros()).OrderBy(l => l.Titulo).ToList();
            if (string.IsNullOrWhiteSpace(_id)) return;
            var emprestimo = (await _biblioteca.ListarEmprestimos()).FirstOrDefault(item => item.Id == _id);
            if (emprestimo is null) { await DisplayAlertAsync("Aviso", "Empréstimo não encontrado.", "OK"); return; }
            TituloPagina.Text = "Editar empréstimo";
            LivroPicker.SelectedItem = ((List<Livro>)LivroPicker.ItemsSource).FirstOrDefault(l => l.Id == emprestimo.LivroId);
            PessoaEntry.Text = emprestimo.Pessoa;
            DataEmprestimoPicker.Date = emprestimo.DataEmprestimo;
            DataDevolucaoPicker.Date = emprestimo.DataPrevistaDevolucao;
            DevolvidoCheck.IsChecked = emprestimo.Devolvido;
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro", ex.Message, "OK"); }
    }

    private async void SalvarClicked(object? sender, EventArgs e)
    {
        if (LivroPicker.SelectedItem is not Livro livro || string.IsNullOrWhiteSpace(PessoaEntry.Text) ||
            (DataDevolucaoPicker.Date ?? DateTime.MinValue) < (DataEmprestimoPicker.Date ?? DateTime.MinValue))
        {
            await DisplayAlertAsync("Validação", "Selecione o livro, informe a pessoa e confira as datas.", "OK");
            return;
        }
        try
        {
            await _biblioteca.SalvarEmprestimo(new Emprestimo
            {
                Id = _id ?? "",
                LivroId = livro.Id,
                LivroTitulo = livro.Titulo,
                Pessoa = PessoaEntry.Text.Trim(),
                DataEmprestimo = DataEmprestimoPicker.Date ?? DateTime.Today,
                DataPrevistaDevolucao = DataDevolucaoPicker.Date ?? DateTime.Today.AddDays(14),
                Devolvido = DevolvidoCheck.IsChecked
            });
            await MensagemService.Mostrar("Empréstimo salvo.");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await DisplayAlertAsync("Erro ao salvar", ex.Message, "OK"); }
    }

    private async void CancelarClicked(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
}
