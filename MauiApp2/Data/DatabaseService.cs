using MauiApp1.Models;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;

public class DatabaseService
{
    private static SQLiteAsyncConnection _database;
    private static string DbPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "mobiledb.db");

    public static async Task InitializeDatabaseAsync()
    {
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(DbPath);
            await _database.ExecuteAsync("PRAGMA foreign_keys = ON;");
            await _database.CreateTableAsync<Users>();
            await _database.CreateTableAsync<Entry>();
            await _database.CreateTableAsync<ReasonList>();
        }
        var existingReasons = await _database.Table<ReasonList>().ToListAsync();
        if (!existingReasons.Any())
        {
            var reasons = new List<ReasonList>
            {
                new ReasonList { ReasonName = "Переустройство/перепланировка помещения в многоквартирном доме" },
                new ReasonList { ReasonName = "Принять на учет гражданина, нуждающимся в жил.помещениях" },
                new ReasonList { ReasonName = "Бытовая характеристика гражданина" },
                new ReasonList { ReasonName = "Справка о составе семьи" },
                new ReasonList { ReasonName = "Выписка из ЕГРН" },
                new ReasonList { ReasonName = "Справка с места жительства умершего" },
                new ReasonList { ReasonName = "Разрешение на захоронение" },
            };
            await _database.InsertAllAsync(reasons);
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