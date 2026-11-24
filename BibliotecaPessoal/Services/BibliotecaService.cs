using BibliotecaPessoal.Models;
using Microsoft.Maui.Networking;

namespace BibliotecaPessoal.Services;

public class BibliotecaService
{
    private const string Raiz = "biblioteca";
    private readonly FirebaseService _firebase;
    private readonly CacheJsonService _cache;

    public BibliotecaService(FirebaseService firebase, CacheJsonService cache)
    {
        _firebase = firebase;
        _cache = cache;
    }

    public Task<List<Livro>> ListarLivros() => Listar<Livro>("livros");
    public Task<List<Categoria>> ListarCategorias() => Listar<Categoria>("categorias");
    public Task<List<Emprestimo>> ListarEmprestimos() => Listar<Emprestimo>("emprestimos");

    public async Task SalvarLivro(Livro livro)
    {
        livro.Id = NovoId(livro.Id);
        await Salvar("livros", livro);
    }

    public async Task SalvarCategoria(Categoria categoria)
    {
        categoria.Id = NovoId(categoria.Id);
        await Salvar("categorias", categoria);
    }

    public async Task SalvarEmprestimo(Emprestimo emprestimo)
    {
        emprestimo.Id = NovoId(emprestimo.Id);
        await Salvar("emprestimos", emprestimo);
    }

    public async Task ExcluirLivro(string id)
    {
        var emprestimos = await ListarEmprestimos();
        if (emprestimos.Any(e => e.LivroId == id && !e.Devolvido))
            throw new InvalidOperationException("Este livro está emprestado. Registre a devolução antes de excluí-lo.");
        await Excluir("livros", id);
    }

    public async Task ExcluirCategoria(string id)
    {
        var livros = await ListarLivros();
        if (livros.Any(l => l.CategoriaId == id))
            throw new InvalidOperationException("Esta categoria está vinculada a um livro e não pode ser excluída.");
        await Excluir("categorias", id);
    }

    public Task ExcluirEmprestimo(string id) => Excluir("emprestimos", id);

    private async Task<List<T>> Listar<T>(string nome) where T : Registro
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var dados = await _firebase.Listar<T>($"{Raiz}/{nome}");
            var lista = dados.Select(item =>
            {
                item.Value.Id = item.Key;
                return item.Value;
            }).ToList();
            await _cache.Salvar(nome, lista);
            return lista;
        }

        var local = await _cache.Ler<T>(nome);
        if (local is null)
            throw new InvalidOperationException("Sem conexão e sem dados salvos neste aparelho. Conecte-se à internet para carregar a biblioteca.");
        return local;
    }

    private async Task Salvar<T>(string nome, T registro) where T : Registro
    {
        ExigirInternet();
        await GarantirCache(nome, registro);
        await _firebase.Salvar($"{Raiz}/{nome}", registro);

        var lista = await _cache.Ler<T>(nome) ?? new List<T>();
        lista.RemoveAll(item => item.Id == registro.Id);
        lista.Add(registro);
        await _cache.Salvar(nome, lista);
    }

    private async Task Excluir(string nome, string id)
    {
        ExigirInternet();
        var existeCache = nome switch
        {
            "livros" => await _cache.Ler<Livro>(nome) is not null,
            "categorias" => await _cache.Ler<Categoria>(nome) is not null,
            _ => await _cache.Ler<Emprestimo>(nome) is not null
        };
        if (!existeCache)
            await AtualizarCache(nome);

        await _firebase.Excluir($"{Raiz}/{nome}", id);
        switch (nome)
        {
            case "livros":
                var livros = await _cache.Ler<Livro>(nome) ?? new List<Livro>();
                livros.RemoveAll(item => item.Id == id);
                await _cache.Salvar(nome, livros);
                break;
            case "categorias":
                var categorias = await _cache.Ler<Categoria>(nome) ?? new List<Categoria>();
                categorias.RemoveAll(item => item.Id == id);
                await _cache.Salvar(nome, categorias);
                break;
            default:
                var emprestimos = await _cache.Ler<Emprestimo>(nome) ?? new List<Emprestimo>();
                emprestimos.RemoveAll(item => item.Id == id);
                await _cache.Salvar(nome, emprestimos);
                break;
        }
    }

    private async Task GarantirCache<T>(string nome, T registro) where T : Registro
    {
        var existe = nome switch
        {
            "livros" => await _cache.Ler<Livro>(nome) is not null,
            "categorias" => await _cache.Ler<Categoria>(nome) is not null,
            _ => await _cache.Ler<Emprestimo>(nome) is not null
        };
        if (!existe)
            await AtualizarCache(nome);
    }

    private async Task AtualizarCache(string nome)
    {
        switch (nome)
        {
            case "livros": await ListarOnline<Livro>(nome); break;
            case "categorias": await ListarOnline<Categoria>(nome); break;
            default: await ListarOnline<Emprestimo>(nome); break;
        }
    }

    private async Task ListarOnline<T>(string nome) where T : Registro
    {
        var dados = await _firebase.Listar<T>($"{Raiz}/{nome}");
        var lista = dados.Select(item => { item.Value.Id = item.Key; return item.Value; }).ToList();
        await _cache.Salvar(nome, lista);
    }

    private static string NovoId(string id) => string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString("N") : id;

    private static void ExigirInternet()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            throw new InvalidOperationException("É necessário estar conectado à internet para salvar ou excluir dados.");
    }
}
