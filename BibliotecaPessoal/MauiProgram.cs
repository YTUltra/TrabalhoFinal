using BibliotecaPessoal.Services;
using BibliotecaPessoal.ViewModels;
using BibliotecaPessoal.Views;
using Microsoft.Extensions.Logging;

namespace BibliotecaPessoal;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(new HttpClient());
        builder.Services.AddSingleton<FirebaseService>();
        builder.Services.AddSingleton<CacheJsonService>();
        builder.Services.AddSingleton<BibliotecaService>();
        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<InicioPage>();
        builder.Services.AddTransient<LivrosPage>();
        builder.Services.AddTransient<CategoriasPage>();
        builder.Services.AddTransient<EmprestimosPage>();
        builder.Services.AddTransient<ConfiguracaoPage>();
        builder.Services.AddTransient<LivroFormPage>();
        builder.Services.AddTransient<CategoriaFormPage>();
        builder.Services.AddTransient<EmprestimoFormPage>();

        builder.Services.AddTransient<InicioViewModel>();
        builder.Services.AddTransient<LivrosViewModel>();
        builder.Services.AddTransient<CategoriasViewModel>();
        builder.Services.AddTransient<EmprestimosViewModel>();
        builder.Services.AddTransient<ConfiguracaoViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
