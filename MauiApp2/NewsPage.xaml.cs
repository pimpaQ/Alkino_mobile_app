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
            Loaded += (s, e) => LoadNews();
        }

        private async void LoadNews()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
    (sender, cert, chain, sslPolicyErrors) => true;
            try
            {
                string accessToken = "3b964a553b964a553b964a556f38b0443533b963b964a555cc1821afaffb2e39442c8ea";
                string apiVersion = "5.131";
                string groupId = "-217342443";

                string url = $"https://api.vk.com/method/wall.get?owner_id={groupId}&count=15&access_token={accessToken}&v={apiVersion}";

                using var client = new HttpClient();
                var response = await Task.Run(() => client.GetStringAsync(url));

                var jsonDoc = JsonDocument.Parse(response);
                var root = jsonDoc.RootElement;

                // Проверка на ошибку API
                if (root.TryGetProperty("error", out var error))
                {
                    throw new Exception($"VK Error: {error.GetProperty("error_msg").GetString()}");
                }

                // Валидация структуры ответа
                if (!root.TryGetProperty("response", out var responseElement) ||
                    !responseElement.TryGetProperty("items", out var posts))
                {
                    throw new Exception("Invalid API response structure");
                }

                VkPosts.Clear();
                foreach (var post in posts.EnumerateArray())
                {
                    string text = post.TryGetProperty("text", out var t) ? t.GetString() : "";
                    int id = post.TryGetProperty("id", out var i) ? i.GetInt32() : 0;
                    string postUrl = $"https://vk.com/wall{groupId}_{post.GetProperty("id").GetInt32()}";
                    string imageUrl = null;

                    // Если это репост, получаем оригинальный текст и вложения
                    JsonElement attachmentsSource = post;
                    if (string.IsNullOrWhiteSpace(text) &&
                        post.TryGetProperty("copy_history", out var copyHistory) &&
                        copyHistory.ValueKind == JsonValueKind.Array &&
                        copyHistory.GetArrayLength() > 0)
                    {
                        var originalPost = copyHistory[0];
                        text = originalPost.TryGetProperty("text", out var textProp) ? textProp.GetString() : "";
                    
                                            attachmentsSource = originalPost;
                    }

                    // Поиск фото или видео
                    if (attachmentsSource.TryGetProperty("attachments", out var attachments))
                    {
                        foreach (var attachment in attachments.EnumerateArray())
                        {
                            if (!attachment.TryGetProperty("type", out var typeProp)) continue;

                            string type = typeProp.GetString();

                            if (type == "photo" && attachment.TryGetProperty("photo", out var photo))
                            {
                                photo = attachment.GetProperty("photo");
                                imageUrl = photo
                                    .GetProperty("sizes")
                                    .EnumerateArray()
                                    .OrderByDescending(size => size.GetProperty("width").GetInt32())
                                    .First()
                                    .GetProperty("url")
                                    .GetString();
                                break;
                            }
                            else if (type == "video")
                            {
                                var video = attachment.GetProperty("video");
                                if (video.TryGetProperty("image", out var images))
                                {
                                    imageUrl = images
                                        .EnumerateArray()
                                        .OrderByDescending(img => img.GetProperty("width").GetInt32())
                                        .First()
                                        .GetProperty("url")
                                        .GetString();
                                    break;
                                }
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

                NewsCollectionView.ItemsSource = VkPosts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось загрузить новости: " + ex.Message, "OK");
            }
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
