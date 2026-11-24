using BibliotecaPessoal.Services;
using BibliotecaPessoal.Models;

namespace BibliotecaPessoal.ViewModels;

public class ConfiguracaoViewModel : BaseViewModel
{
    private readonly FirebaseService _firebase;
    private string _url = Preferences.Default.Get("FirebaseDatabaseUrl", "");
    private string _status = "Informe a URL HTTPS do Realtime Database.";

    public string Url { get => _url; set { _url = value; Avisar(); } }
    public string Status { get => _status; set { _status = value; Avisar(); } }
    public Command SalvarCommand { get; }
    public Command TestarCommand { get; }

    public ConfiguracaoViewModel(FirebaseService firebase)
    {
        _firebase = firebase;
        SalvarCommand = new Command(Salvar);
        TestarCommand = new Command(async () => await Testar());
    }

    private void Salvar()
    {
        if (!Uri.TryCreate(Url.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            Status = "Digite uma URL HTTPS válida.";
            return;
        }
        Preferences.Default.Set("FirebaseDatabaseUrl", uri.ToString().TrimEnd('/'));
        Status = "URL salva neste aparelho.";
    }

    private async Task Testar()
    {
        try
        {
            Salvar();
            if (Status != "URL salva neste aparelho.") return;
            await _firebase.Listar<Livro>("biblioteca/livros");
            Status = "Conexão com o Firebase realizada.";
        }
        catch (Exception ex) { Status = ex.Message; }
    }
}
