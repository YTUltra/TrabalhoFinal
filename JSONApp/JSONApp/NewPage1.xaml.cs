using System.Text.Json;

namespace JSONApp;

public partial class NewPage1 : ContentPage
{
    string ListClienteJSON;

    List<Cliente> ListaCliente = new List<Cliente>
    {
        new Cliente { Nome = "Rafael César Michalczuk", AnoNascimento = 2005, CPF = "041.213.650-19"},
        new Cliente { Nome = "João Henrique Antoniazzi", AnoNascimento = 2005, CPF = "123.456.789-09"},
        new Cliente { Nome = "Maria Eduarda Falkoski", AnoNascimento = 2003, CPF = "214.215.190-00"}
    };

    private static readonly HttpClient _httpClient = new HttpClient();

	public NewPage1()
	{
		InitializeComponent();

        BindingContext = this;

        lvClientes.ItemsSource = ListaCliente;
	}

    private void btAdicionarCliente_Clicked(object sender, EventArgs e)
    {
        Cliente cli = new Cliente
        {
            Nome = txNome.Text,
            AnoNascimento = Convert.ToInt32(txAno.Text),
            CPF = txCPF.Text
        };

        ListaCliente.Add(cli);
        lvClientes.ItemsSource = null;
        lvClientes.ItemsSource = ListaCliente;

        try 
        { 
            ListClienteJSON = JsonSerializer.Serialize(ListaCliente);

            lbJSON.Text = ListClienteJSON;
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("ERRO", $"Falha ao serializar para JSON: {ex.Message}", "OK");
        }
    }

    private void btJSONtoList_Clicked(object sender, EventArgs e)
    {
        List<Cliente> NovaListaClientes = new List<Cliente>();

        try
        {
            NovaListaClientes = JsonSerializer.Deserialize<List<Cliente>>(ListClienteJSON);

            string listaClientes = string.Join("\n", NovaListaClientes.Select(c => $"Nome: {c.Nome}, Ano: {c.AnoNascimento}, CPF: {c.CPF}"));

            DisplayAlertAsync("Clientes do JSON", listaClientes, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlertAsync("ERRO", $"Falha ao deserializar para JSON: {ex.Message}", "OK");
        }
    }

    private async void btWebJSONtoList_Clicked(object sender, EventArgs e)
    {
        List<Cliente> ListaClientesJSON = new List<Cliente>();

        try
        {
            lbStatus.Text = "Status: Baixando dados...";
            lbStatus.TextColor = Colors.Blue;

            string url = "https://api.npoint.io/f9714858e3a905758755";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            ListaClientesJSON = JsonSerializer.Deserialize<List<Cliente>>(jsonString, options);

            lbStatus.Text = "Status: Dados da Web carregados com sucesso!";
            lbStatus.TextColor = Colors.Green;

            lvClientes.ItemsSource = null;
            lvClientes.ItemsSource = ListaClientesJSON;
        }
        catch (Exception ex)
        {
            lbStatus.Text = "Status: Erro ao carregar dados da Web!";
            lbStatus.TextColor = Colors.Red;

            DisplayAlertAsync("Erro", $"Falha ao desserializar JSON. {ex.Message}", "OK");
        }
    }
}