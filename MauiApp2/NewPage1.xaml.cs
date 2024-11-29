using MauiApp1.Data;
using MauiApp1.Models;
using System.Collections.ObjectModel;

namespace MauiApp1
{
    public partial class NewPage1 : ContentPage
    {
        public string UserName { get; set; }
        public ObservableCollection<Entry> UserEntries { get; set; }
        public Entry NearestEntry { get; set; }
        public NewPage1()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            
            // Загрузка данных
            LoadUserData();
        }
        
        private async void LoadUserData()
        {
            // Получаем текущего пользователя
            var userId = Session.CurrentUserId; // Идентификатор пользователя из сессии
            var currentUser = await DatabaseService.Database.Table<Users>()
                                      .FirstOrDefaultAsync(u => u.UserID == userId);

            // Устанавливаем имя пользователя
            UserName = $"Добро пожаловать, {currentUser.Name}!";
            OnPropertyChanged(nameof(UserName));

            // Получаем записи текущего пользователя, отсортированные по дате и времени
            var entries = await DatabaseService.Database.Table<Entry>()
                                   .Where(e => e.UserID == userId)
                                   .OrderByDescending(e => e.Date)
                                   .ThenBy(e => e.Time)
                                   .ToListAsync();

            foreach (var entry in entries)
            {
                // Загружаем связанные данные из ReasonList
                entry.Reason = await DatabaseService.Database.Table<ReasonList>()
                                          .FirstOrDefaultAsync(r => r.ReasonListId == entry.ReasonID);
            }

            // Устанавливаем ближайшую запись и остальные
            if (entries.Any())
            {
                var nearestEntry = entries.First(); // Ближайшая запись
                entries.RemoveAt(0); // Удаляем ближайшую из списка остальных

                // Устанавливаем данные для привязки
                UserEntries = new ObservableCollection<Entry>(entries);
                NearestEntry = nearestEntry;
                
            }
            else
            {
                
                
            }
            OnPropertyChanged(nameof(UserEntries));
            OnPropertyChanged(nameof(NearestEntry));
        }
        
        private async void profileBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
        private async void ToEntryPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntryPage());

        }

        private async void EntrylistPage_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserEntryPage());
        }
    }
}

