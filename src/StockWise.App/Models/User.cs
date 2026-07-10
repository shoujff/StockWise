using System.Collections.Generic;

namespace StockWise.App.Models;
public class User
{
    public int Id {get; set;}
    public string FirstName {get;set;}= string.Empty;
    public string LastName {get; set;}= string.Empty;
    public string Login{get;set;}= string.Empty;
    public string PasswordHash {get;set;}= string.Empty;

    public string Role { get; set; } = string.Empty;

    public string GetFullName() => $"{FirstName} {LastName}";

   
    public List<Transaction> Transactions { get; set; } = new();
}