using SQLite;
using MauiApp1.Models;

public class Users
{
    [PrimaryKey,AutoIncrement]
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }
    public string PassportSerya { get; set; }
    public string PassportNum { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Accesibility { get; set; }

}
