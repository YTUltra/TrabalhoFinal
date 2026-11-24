using BibliotecaPessoal.ViewModels;

namespace BibliotecaPessoal.Views;

public partial class ConfiguracaoPage : ContentPage
{
    public ConfiguracaoPage(ConfiguracaoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
