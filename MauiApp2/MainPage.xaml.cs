using MauiApp1.Data;
using MauiApp1.Models;
using MauiApp1.Resources.Strings;
using MauiApp1.VKApi;
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
            LoadUserData();
            BindingContext = this;
        }

        private async void LoadUserData()
        {
            var userId = Session.CurrentUserId; // ������������� ������������ �� ������
            var currentUser = await DatabaseService.Database.Table<Users>()
                                      .FirstOrDefaultAsync(u => u.UserID == userId);
            string welcomeText = AppResources.Welcome;
            UserName = welcomeText + ", " + currentUser.Name;

            OnPropertyChanged(nameof(UserName));
            // �������� ������ �������� ������������, ��������������� �� ���� � �������
            var entries = await DatabaseService.Database.Table<Entry>()
                                   .Where(e => e.UserID == userId)
                                   .OrderBy(e => e.Date)
                                   .ThenBy(e => e.Time)
                                   .ToListAsync();

            foreach (var entry in entries)
            {
                // ��������� ��������� ������ �� ReasonList
                entry.Reason = await DatabaseService.Database.Table<ReasonList>()
                                          .FirstOrDefaultAsync(r => r.ReasonListId == entry.ReasonID);
            }

            // ������������� ��������� ������ � ���������
            if (entries.Any())
            {
                var nearestEntry = entries.First(); // ��������� ������
                entries.RemoveAt(0); // ������� ��������� �� ������ ���������

                nearestEntry.Reason.ReasonName = Entry.GetLocalizedReason(nearestEntry.Reason.ReasonName);

                // ������������� ������ ��� ��������
                UserEntries = new ObservableCollection<Entry>(entries);
                NearestEntry = nearestEntry;

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

        private async void newsBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewsPage());
        }

        private async void vkBtn_Clicked(object sender, EventArgs e)
        {
            string vkUrl = "https://vk.com/public217342443";
            await Launcher.OpenAsync(vkUrl);
        }

        private async void siteBtn_Clicked(object sender, EventArgs e)
        {
            string siteUrl = "https://cp-alkino.ru";
            await Launcher.OpenAsync(siteUrl);
        }

        private async void gosBtn_Clicked(object sender, EventArgs e)
        {
            string gosUrl = "https://pos.gosuslugi.ru/form/?opaId=329659&utm_source=vk&utm_medium=80&utm_campaign=1020201202218";
            await Launcher.OpenAsync(gosUrl);
        }
    }
}

