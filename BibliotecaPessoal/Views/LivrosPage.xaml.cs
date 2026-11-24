using BibliotecaPessoal.ViewModels;

namespace BibliotecaPessoal.Views;

public partial class LivrosPage : ContentPage
{
    private readonly LivrosViewModel _viewModel;

    public LivrosPage(LivrosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.Atualizar();
    }
}
