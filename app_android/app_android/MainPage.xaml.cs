namespace app_android
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void calcular_Clicked(object sender, EventArgs e)
        {
            float fValorX = Convert.ToSingle(valorX.Text);
            float fValorY = Convert.ToSingle(valorY.Text);
            float fResultado = fValorX + fValorY;

            string nome = await DisplayPromptAsync("Nome", "Digite o seu nome: ", "OK");
            await DisplayAlertAsync("Olá " + nome, "O resultado é: " + fResultado, "OK", "Cancelar");

            resultado.Text = fResultado.ToString();


        }
    }
}
