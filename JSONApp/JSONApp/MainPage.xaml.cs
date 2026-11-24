namespace JSONApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void btPage1_Clicked(object sender, EventArgs e)
        {
            NewPage1 page1 = new NewPage1();
            Navigation.PushAsync(page1);
        }

        private void btPage2_Clicked(object sender, EventArgs e)
        {
            NewPage2 page2 = new NewPage2();
            Navigation.PushAsync(page2);
        }
    }
}
