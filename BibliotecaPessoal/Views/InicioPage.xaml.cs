using BibliotecaPessoal.ViewModels;

namespace BibliotecaPessoal.Views;

public partial class InicioPage : ContentPage
{
    private readonly InicioViewModel _viewModel;

    public InicioPage(InicioViewModel viewModel)
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
