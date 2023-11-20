namespace FoodXYZ.Models;

public class User
{
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
}