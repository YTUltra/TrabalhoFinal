using BibliotecaPessoal.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BibliotecaPessoal;

public partial class AppShell : Shell
{
    public AppShell(IServiceProvider services)
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LivroFormPage), typeof(LivroFormPage));
        Routing.RegisterRoute(nameof(CategoriaFormPage), typeof(CategoriaFormPage));
        Routing.RegisterRoute(nameof(EmprestimoFormPage), typeof(EmprestimoFormPage));

        var biblioteca = new FlyoutItem { Title = "Minha biblioteca", Route = "biblioteca" };
        biblioteca.Items.Add(CriarAba("Início", "inicio", () => services.GetRequiredService<InicioPage>()));
        biblioteca.Items.Add(CriarAba("Livros", "livros", () => services.GetRequiredService<LivrosPage>()));
        biblioteca.Items.Add(CriarAba("Categorias", "categorias", () => services.GetRequiredService<CategoriasPage>()));
        biblioteca.Items.Add(CriarAba("Empréstimos", "emprestimos", () => services.GetRequiredService<EmprestimosPage>()));
        Items.Add(biblioteca);

        var configuracao = new FlyoutItem { Title = "Configuração", Route = "configuracao" };
        configuracao.Items.Add(CriarAba("Firebase", "firebase", () => services.GetRequiredService<ConfiguracaoPage>()));
        Items.Add(configuracao);
    }

    private static Tab CriarAba(string titulo, string rota, Func<Page> criarPagina)
    {
        var tab = new Tab { Title = titulo, Route = $"aba-{rota}" };
        tab.Items.Add(new ShellContent { Title = titulo, Route = rota, ContentTemplate = new DataTemplate(criarPagina) });
        return tab;
    }
}
