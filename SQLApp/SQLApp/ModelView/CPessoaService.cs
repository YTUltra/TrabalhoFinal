using System;
using System.Collections.Generic;
using System.Text;

using SQLite;
using SQLApp.Model;
using System.Collections.ObjectModel;

namespace SQLApp.ModelView
{
    public class CPessoaService
    {
        // Controlar o acesso ao banco de dados SQLite
        private SQLiteAsyncConnection _db;

        // Lista de pessoas vindas do banco de dados
        private IEnumerable<CPessoa> _pessoas;

        public ObservableCollection<CPessoa> PessoasFiltradas { get; private set; }

        // Construtor
        public CPessoaService() 
        {
            Init();
        }

        public async Task Init()
        {
            // Se a conexão com o banco de dados já foi inicializada,
            // não faz nada
            if (_db != null)
                return;

            // Inicializa a conexão com o banco de dados SQLite 

            // Definir o nome do arquivo do banco de dados SQLite
            const string DatabaseFilename = "pessoas.db3";

            // Define Flags para o SQLite
            const SQLiteOpenFlags Flags =
                // Abre o banco de dados para leitura e escrita
                SQLite.SQLiteOpenFlags.ReadWrite |
                // Cria o banco de dados se ele não existir
                SQLite.SQLiteOpenFlags.Create |
                // Permite acesso compartilhado ao banco de dados
                SQLite.SQLiteOpenFlags.SharedCache;

            // Cria a conexão com o banco de dados SQLite no ANDROID
            string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, 
                 DatabaseFilename);

            // Cria a conexão com o banco de dados SQLite
            _db = new SQLiteAsyncConnection(DatabasePath, Flags);

            // Cria a(s) tabela(s) no banco de dados SQLite
            await _db.CreateTableAsync<CPessoa>();
            // ...
        }

        // Método para inserir uma pessoa no banco de dados SQLite
        public Task<int> InsertPessoa(CPessoa p) => _db.InsertAsync(p);

        // Método para atualizar uma pessoa no banco de dados SQLite
        public Task<int> UpdatePessoa(CPessoa p) => _db.UpdateAsync(p);

        // Método para excluir uma pessoa do banco de dados SQLite
        public Task<int> DeletePessoa(CPessoa p) => _db.DeleteAsync(p);

        // Método para obter todas as pessoas do banco de dados SQLite
        public Task<List<CPessoa>> GetPessoas() => _db.Table<CPessoa>().ToListAsync();

        // Método para consultar pessoas no banco de dados SQLite
        // com base em um filtro (ID)
        public Task<List<CPessoa>> ConsultarID(string query)
        {
            string sql = $"SELECT * FROM CPessoa WHERE Id LIKE '%{query}%'";
            return _db.QueryAsync<CPessoa>(sql);
        }

        // Método para consultar pessoas  por Nome no banco de dados SQLite
        // Usando no componente SearchBar do .NET MAUI
        public Task<List<CPessoa>> ConsultarNome(string query)
        {
            string sql = $"SELECT * FROM CPessoa WHERE Nome LIKE '%{query}%'";
            return _db.QueryAsync<CPessoa>(sql);
        }

        public void OrdenarPorNome(bool crescente = true)
        {
            // Se a lista de pessoas ainda não foi carregada do banco de dados, carrega-a
            if (_pessoas == null)
                _pessoas = _db.Table<CPessoa>().ToListAsync().Result;

            // Se a lista de pessoas estiver vazia, não faz nada
            if (_pessoas.Count() == 0)
                return;

            // Ordena a lista de pessoas por nome, em ordem crescente ou decrescente 
            var ordenadas = crescente
                ? _pessoas.OrderBy(p => p.Nome).ToList()
                : _pessoas.OrderByDescending(p => p.Nome).ToList();

            // Atualiza a lista de pessoas filtradas
            PessoasFiltradas = new ObservableCollection<CPessoa>(ordenadas);
        }


    }
}
