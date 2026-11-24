using Microsoft.Extensions.DependencyInjection;

namespace JSONApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var MainPage = new NavigationPage(new MainPage());
            MainPage.BarBackgroundColor = Colors.Black;
            MainPage.BarTextColor = Colors.White;


            return new Window(MainPage);
        }
    }
}