using System.Globalization;

namespace MauiApp1
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var culture = new CultureInfo("ba"); // Для башкирского языка
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            MainPage = new NavigationPage(new MainPage());
        }
        protected override async void OnStart()
        {
            await DatabaseService.InitializeDatabaseAsync();
        }
    }
}
