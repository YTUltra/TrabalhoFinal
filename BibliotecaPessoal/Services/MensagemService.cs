namespace BibliotecaPessoal.Services;

public static class MensagemService
{
    public static async Task Mostrar(string mensagem)
    {
#if ANDROID
        await MainThread.InvokeOnMainThreadAsync(() =>
            Android.Widget.Toast.MakeText(Android.App.Application.Context, mensagem, Android.Widget.ToastLength.Short)?.Show());
#else
        var pagina = Application.Current?.Windows.FirstOrDefault()?.Page;
        if (pagina is not null)
            await pagina.DisplayAlertAsync("Biblioteca Pessoal", mensagem, "OK");
#endif
    }
}
