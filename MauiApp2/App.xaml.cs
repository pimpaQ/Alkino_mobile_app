using System.Globalization;

namespace MauiApp1
{
    public partial class App : Application
    {
        public static LocalizationResourceManager LocalizationManager { get; } = new LocalizationResourceManager();
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }
        void SetCulture()
        {
            string lang = Preferences.Get("AppLanguage", "ru"); // Загружаем язык из настроек
            CultureInfo culture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        protected override async void OnStart()
        {
            await DatabaseService.InitializeDatabaseAsync();
        }
    }
}
