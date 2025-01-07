using MauiApp1.Models;
using MauiApp1.VKApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiApp1
{
    public partial class NewsPage : ContentPage
    {
        private List<VkPost> VkPosts { get; set; } = new List<VkPost>();

        public NewsPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadNews();
        }

        private async void LoadNews()
        {
            string accessToken = "3b964a553b964a553b964a556f38b0443533b963b964a555cc1821afaffb2e39442c8ea";
            string apiVersion = "5.131";
            string groupId = "-217342443"; // ID группы ВК (отрицательное значение для паблика)

            string url = $"https://api.vk.com/method/wall.get?owner_id={groupId}&count=5&access_token={accessToken}&v={apiVersion}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Разбираем JSON-ответ
            var jsonDoc = JsonDocument.Parse(response);
            var posts = jsonDoc.RootElement.GetProperty("response").GetProperty("items");

            VkPosts.Clear();

            foreach (var post in posts.EnumerateArray())
            {
                string text = post.GetProperty("text").GetString();
                string postUrl = $"https://vk.com/wall{groupId}_{post.GetProperty("id").GetInt32()}";

                string imageUrl = null;
                if (post.TryGetProperty("attachments", out var attachments))
                {
                    foreach (var attachment in attachments.EnumerateArray())
                    {
                        if (attachment.GetProperty("type").GetString() == "photo")
                        {
                            var photo = attachment.GetProperty("photo");
                            imageUrl = photo
                                .GetProperty("sizes")
                                .EnumerateArray()
                                .OrderByDescending(size => size.GetProperty("width").GetInt32())
                                .First()
                                .GetProperty("url")
                                .GetString();
                            break;
                        }
                    }
                }

                VkPosts.Add(new VkPost
                {
                    Text = text,
                    ImageUrl = imageUrl,
                    PostUrl = postUrl
                });
            }

            // Устанавливаем данные в CollectionView
            NewsCollectionView.ItemsSource = VkPosts;
        }

        private async void OpenPost_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter is string postUrl)
            {
                bool confirm = await DisplayAlert("Открыть новость", "Вы уверены, что хотите перейти?", "Да", "Нет");
                if (confirm)
                {
                    await Launcher.OpenAsync(postUrl);
                }
            }
        }

        private async void mainPageBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewPage1());
        }

        private async void EntryPageBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EntryPage());
        }

        private async void profileBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
    }
}
