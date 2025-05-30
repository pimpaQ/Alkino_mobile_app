using MauiApp1.Data;
using MauiApp1.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage() 
        {
            InitializeComponent();
            
        }
        
        private void EnterButton(object sender, EventArgs e)
        {
            RegistrView.IsVisible = false;
            RegistrView.InputTransparent = true;
            EnterView.IsVisible = true;

            AuthorizationBtn.Background = Colors.White;
            AuthorizationBtn.TextColor = Colors.Black;

            RegistrationBtn.Background = Colors.Black;
            RegistrationBtn.TextColor = Colors.White;
        }

        private void RegistrationButton(object sender, EventArgs e)
        {
            RegistrView.IsVisible = true;
            RegistrView.InputTransparent = false;
            EnterView.IsVisible = false;

            AuthorizationBtn.Background = Colors.Black;
            AuthorizationBtn.TextColor = Colors.White;

            RegistrationBtn.Background = Colors.White;
            RegistrationBtn.TextColor = Colors.Black;

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(firstName.Text) ||
        string.IsNullOrWhiteSpace(Name.Text) ||
        string.IsNullOrWhiteSpace(Patronymic.Text) ||
        string.IsNullOrWhiteSpace(Phone.Text) ||
        string.IsNullOrWhiteSpace(passWord.Text))
            {
                await DisplayAlert("Ошибка", "Все поля должны быть заполнены.", "ОК");
                return;
            }

            if (!Regex.IsMatch(Phone.Text, @"^\+7\d{10}$"))
            {
                await DisplayAlert("Ошибка", "Введите номер телефона в формате +7XXXXXXXXXX.", "ОК");
                return;
            }

            var existingUser = await DatabaseService.Database.Table<Users>().FirstOrDefaultAsync
                (u =>u.Phone == Phone.Text || (u.FirstName == firstName.Text && u.Name == Name.Text && u.Patronymic == Patronymic.Text));
            if (existingUser != null)
            {
                await DisplayAlert("Ошибка", "Пользователь с такими данными уже зарегистрирован.", "ОК");
                return;
            }
            string salt = PasswordHasher.GenerateSalt();
            string hashedPassword = PasswordHasher.HashPassword(passWord.Text, salt);
            var user = new Users
            {
                FirstName = firstName.Text,
                Name = Name.Text,
                Patronymic = Patronymic.Text,
                Phone = Phone.Text,
                Password = hashedPassword,
                Salt = salt,
                Accesibility = 0,

            };

            await DatabaseService.Database.InsertAsync(user);
            var createdUser = await DatabaseService.Database.Table<Users>()
                               .OrderByDescending(u => u.UserID)
                               .FirstOrDefaultAsync();
            if (createdUser != null)
            {
                Session.CurrentUserId = createdUser.UserID;
            }
            await DisplayAlert("Успех", "Пользователь зарегистрирован!", "ОК");
            Navigation.InsertPageBefore(new NewPage1(), this);
            await Navigation.PopAsync();
        }

        private async void EnterToApps_Clicked(object sender, EventArgs e)
        {
            var user = await DatabaseService.Database.Table<Users>()
                  .FirstOrDefaultAsync(u => u.Name == email.Text);

            if (user != null)
            {
                string inputHash = PasswordHasher.HashPassword(pAssword.Text, user.Salt);
                if (user.Password == inputHash)
                {
                    // Успешный вход
                    Session.CurrentUserId = user.UserID;
                    if (user.Accesibility == 1) // Если пользователь администратор
                    {
                        Navigation.InsertPageBefore(new AdminPage(), this);
                    }
                    else // Если обычный пользователь
                    {
                        await DisplayAlert("Успех", $"Добро пожаловать, {user.Name}!", "ОК");
                        Navigation.InsertPageBefore(new NewPage1(), this);
                    }

                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Ошибка", "Неверный пароль!", "ОК");
                }          
            }
            else
            {
                await DisplayAlert("Ошибка", "Пользователь не найден!", "ОК");
            }
        }
    }

}
