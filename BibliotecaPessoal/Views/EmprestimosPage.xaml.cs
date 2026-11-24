using BibliotecaPessoal.ViewModels;

namespace BibliotecaPessoal.Views;

public partial class EmprestimosPage : ContentPage
{
    private readonly EmprestimosViewModel _viewModel;

    public EmprestimosPage(EmprestimosViewModel viewModel)
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
