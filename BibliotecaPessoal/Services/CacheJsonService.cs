using System.Text.Json;
using BibliotecaPessoal.Models;

namespace BibliotecaPessoal.Services;

public class CacheJsonService
{
    private readonly JsonSerializerOptions _opcoes = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public async Task<List<T>?> Ler<T>(string nome) where T : Registro
    {
        var caminho = Caminho(nome);
        if (!File.Exists(caminho))
            return null;

        var json = await File.ReadAllTextAsync(caminho);
        return JsonSerializer.Deserialize<List<T>>(json, _opcoes) ?? new List<T>();
    }

    public async Task Salvar<T>(string nome, List<T> registros) where T : Registro
    {
        var json = JsonSerializer.Serialize(registros, _opcoes);
        await File.WriteAllTextAsync(Caminho(nome), json);
    }

    private static string Caminho(string nome) => Path.Combine(FileSystem.AppDataDirectory, $"{nome}.json");
}
