using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BibliotecaPessoal.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Avisar([CallerMemberName] string? nome = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
    }
}
