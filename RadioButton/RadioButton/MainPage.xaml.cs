using Org.Json;
using System.Text.Json;

namespace RadioButton
{
    public partial class MainPage : ContentPage
    {
        private CurrencyConverter _cc;
        public MainPage()
        {
            InitializeComponent();
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var rb = sender as Microsoft.Maui.Controls.RadioButton;


            if (rb.Content.Equals("Dolar"))
            {
                lbValorConvertido.Text =
                    String.Format("{0:C2}", _cc.ConvertValue(Convert.ToDecimal(txValor.Text), "BRL", "USD"));
            }

            if (rb.Content.Equals("Euro"))
            {
                lbValorConvertido.Text =
                    String.Format("{0:C2}", _cc.ConvertValue(Convert.ToDecimal(txValor.Text), "BRL", "EUR"));
            }

            if (rb.Content.Equals("Pesos"))
            {
                lbValorConvertido.Text =
                    String.Format("{0:C2}", _cc.ConvertValue(Convert.ToDecimal(txValor.Text), "BRL", "ARS"));
            }
        }

        private void pickFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
        }

        private void btConverterPicker_Clicked(object sender, EventArgs e)
        {
            decimal amount = Convert.ToDecimal(txValor.Text);

            string from = pickFrom.SelectedItem as string;
            string to = pickTo.SelectedItem as string;

            decimal convertedValue = _cc.ConvertValue(amount, from, to);

            lbValorConvertidoPicker.Text =
                String.Format("{0:C2}", convertedValue);
        }
    }
}
