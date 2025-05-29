using MauiApp1.Models;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;

public class DatabaseService
{
    private static SQLiteAsyncConnection _database;
    public static string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mobiledb.db");

    public static async Task InitializeDatabaseAsync()
    {
        var user = new Users
        {
            FirstName = "Валиуллина",
            Name = "Зульфира",
            Patronymic = "Дамировна",
            Phone = "+79875663214",
            Password = "12345",
            Accesibility = 1,

        };
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(DbPath);
            await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");
            await _database.CreateTableAsync<Users>();
            await _database.CreateTableAsync<Entry>();
            await _database.CreateTableAsync<ReasonList>();
        }
        var predefinedReasons = new List<string>
        {
            "Переустройство/перепланировка помещения в многоквартирном доме",
            "Принять на учет гражданина, нуждающимся в жил.помещениях",
            "Бытовая характеристика гражданина",
            "Справка о составе семьи",
            "Выписка из ЕГРН",
            "Справка с места жительства умершего",
            "Разрешение на захоронение",
            "Другое"
        };

        foreach (var reasonName in predefinedReasons)
        {
            var exists = await _database.Table<ReasonList>().FirstOrDefaultAsync(r => r.ReasonName == reasonName);
            if (exists == null)
            {
                await _database.InsertAsync(new ReasonList { ReasonName = reasonName });
            }
        }
        await RemoveExpiredEntriesAsync();
    }
    private static async Task RemoveExpiredEntriesAsync()
    {
        var currentDateTime = DateTime.Now;

        // Удаляем просроченные записи через критерий
        await _database.ExecuteAsync("DELETE FROM Entry WHERE Date || ' ' || Time < ?", currentDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
    }
    public static SQLiteAsyncConnection Database => _database;
}