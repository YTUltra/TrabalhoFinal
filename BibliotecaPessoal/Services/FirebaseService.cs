using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BibliotecaPessoal.Models;

namespace BibliotecaPessoal.Services;

public class FirebaseService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _opcoes = new() { PropertyNameCaseInsensitive = true };

    public FirebaseService(HttpClient http)
    {
        _http = http;
    }

    public async Task<Dictionary<string, T>> Listar<T>(string caminho) where T : Registro
    {
        using var resposta = await _http.GetAsync(CriarUrl(caminho));
        await VerificarResposta(resposta);
        var json = await resposta.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json) || json == "null")
            return new Dictionary<string, T>();

        using var documento = JsonDocument.Parse(json);
        if (documento.RootElement.ValueKind == JsonValueKind.Object &&
            documento.RootElement.TryGetProperty("error", out var erro))
            throw new InvalidOperationException($"O Firebase recusou a consulta: {erro.GetString()}");

        return JsonSerializer.Deserialize<Dictionary<string, T>>(json, _opcoes)
            ?? new Dictionary<string, T>();
    }

    public async Task Salvar<T>(string caminho, T registro) where T : Registro
    {
        using var resposta = await _http.PutAsJsonAsync(CriarUrl(caminho + "/" + registro.Id), registro);
        await VerificarResposta(resposta);
    }

    public async Task Excluir(string caminho, string id)
    {
        using var resposta = await _http.DeleteAsync(CriarUrl(caminho + "/" + id));
        await VerificarResposta(resposta);
    }

    private static string CriarUrl(string caminho)
    {
        var baseUrl = Preferences.Default.Get("FirebaseDatabaseUrl", "").Trim().TrimEnd('/');
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Configure a URL HTTPS do Realtime Database na página Configuração.");

        var caminhoSeguro = string.Join("/", caminho.Split('/').Select(Uri.EscapeDataString));
        return $"{baseUrl}/{caminhoSeguro}.json";
    }

    private static async Task VerificarResposta(HttpResponseMessage resposta)
    {
        if (resposta.IsSuccessStatusCode)
            return;

        var detalhe = await resposta.Content.ReadAsStringAsync();
        if (resposta.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            throw new InvalidOperationException("O Firebase negou o acesso. Verifique as regras do Realtime Database.");

        throw new HttpRequestException($"Erro do Firebase ({(int)resposta.StatusCode}): {detalhe}");
    }
}
